using whateverthefuck.src.model;
using whateverthefuck.src.util;
using System.Threading;
using whateverthefuckserver.network;
using System.Collections.Generic;
using whateverthefuck.src.network.messages;
using System.Linq;
using whateverthefuck.src.model.entities;
using System;
using whateverthefuck.src.network;
using whateverthefuckserver.storage;
using whateverthefuckserver.users;

namespace whateverthefuckserver.gameserver
{
    class GameServer
    {
        public GameState GameState { get; private set; }
        public IStorage Storage { get; private set; }
        private Timer TickTimer;
        private List<User> PlayingUsers = new List<User>();
        object PlayersLock = new object();

        private List<GameEvent> PendingEvents = new List<GameEvent>();
        object EventsLock = new object();

        private SyncMessageBody SyncCity;

        private SpawnCity SpawnCity { get; }

        public GameServer()
        {
            GameState = new GameState();
            TickTimer = new Timer((_) => Tick(), null, 0, 10);

            new Thread((ob) =>
            {
                Storage = new DummyStorage();
                try
                {
                    Storage = new Mongo();
                }
                catch (Exception)
                {
                }
            }).Start();

            SpawnCity = new SpawnCity(GameState, es => PendEvents(es));
            SpawnCity.SpawnWorld();
            SpawnCity.SpawnMob();
        }

        private void Tick()
        {
            List<GameEvent> sendEvents;

            lock (EventsLock)
            {
                // broken?
                GameState.HandleGameEvents(PendingEvents);
                sendEvents = new List<GameEvent>(PendingEvents);
                var message = new UpdateGameStateMessage(GameState.StepCounter, sendEvents);
                SendMessageToAllPlayers(message);
                PendingEvents.Clear();
            }


            GameState.Step();

            var syncRecord = GameState.GenerateSyncRecord();

            if (syncRecord.Tick % 100 == 1)
            {
                SyncCity = syncRecord;
            }
        }

        internal void HandleEventRequests(List<GameEvent> events)
        {
            PendEvents(events);
        }

        public void AddUser(User user)
        {
            IEnumerable<GameEvent> createEvents;
            IEnumerable<GameEvent> movementEvents;

            lock (EventsLock)
            {
                createEvents = GameState.AllEntities.Select(e => new CreateEntityEvent(e));
                movementEvents = GameState.AllEntities.Where(e => e.Movements.IsMoving).Select(e => new UpdateMovementEvent(e));
            }

            var events = createEvents.Concat(movementEvents).ToArray();


            lock (PlayersLock)
            {
                user.PlayerConnection.SendMessage(new UpdateGameStateMessage(0, events));
                PlayingUsers.Add(user);
            }

            SpawnPlayerCharacter(user);
            Logging.Log("Added player");
        }

        public void RemoveUser(User user)
        {
            lock (PlayersLock)
            {
                PlayingUsers.Remove(user);
            }
            var hero = GameState.GetEntityById(user.HeroIdentifier.Id);
            if (hero != null)
            {
                GameEvent re = new DestroyEntityEvent(hero);
                PendEvents(re);
            }
        }

        public void SpawnLootForPlayer(GameEntity dead, GameEntity killer)
        {
            if (killer is PlayerCharacter)
            {
                // @fix for christ sake just store the PlayerCharacter in the user so we don't end up looping through the entire server every
                // time someone kills something and also this has a very real risk of breaking regardless of what the error message says
                // t ribbe
                User killerx = null;

                lock (PlayersLock)
                {
                    foreach (var player in PlayingUsers)
                    {
                        if (player.HeroIdentifier.Id == killer.Identifier.Id)
                        {
                            killerx = player;
                            break;
                        }
                    }
                }

                if (killerx == null)
                {
                    Logging.Log("this will never happen so you will never see this.", Logging.LoggingLevel.Error);
                }

                Item item = new Item(ItemType.BronzeDagger, 20, Rarity.Epic, 
                    new ItemBonus(ItemBonus.BonusType.Test4, 20),
                    new ItemBonus(ItemBonus.BonusType.Test1, -4)
                    );
                CreateLootMessage message = new CreateLootMessage(dead, item);
                killerx.PlayerConnection.SendMessage(message);
            }
        }

        public bool InSync(int tick, long hash)
        {
            var rt = (SyncCity.Tick, SyncCity.Hash) == (tick, hash);

            if (!rt)
            {
                Logging.Log("Disagreed on sync");
                Logging.Log(string.Format("Tick; expected:'{0}', got'{1}'", SyncCity.Tick, tick));
                Logging.Log(string.Format("Hash; expected:'{0}', got'{1}'", SyncCity.Hash, hash));
            }
            else
            {
                Logging.Log("Agreed on sync");
            }

            return rt;
        }

        private void SpawnPlayerCharacter(User user)
        {
            var createEvent = SpawnCity.SpawnHero();

            user.HeroIdentifier = new EntityIdentifier(createEvent.Id);
            
            // send message to user granting control to created character
            user.PlayerConnection.SendMessage(new GrantControlMessage(createEvent.Id));
        }

        private void SendMessageToAllPlayers(WhateverthefuckMessage message)
        {
            // todo we encode the message seperately for each client
            lock (PlayersLock)
            {
                foreach (var user in PlayingUsers)
                {
                    user.PlayerConnection.SendMessage(message);
                }
            }
        }

        private void PendEvents(IEnumerable<GameEvent> es)
        {
            PendEvents(es.ToArray());
        }

        private void PendEvents(params GameEvent[] es)
        {
            lock (EventsLock)
            {
                PendingEvents.AddRange(es);
            }
        }

        
    }
}
