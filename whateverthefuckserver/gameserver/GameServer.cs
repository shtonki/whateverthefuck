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
using System.Diagnostics;

namespace whateverthefuckserver.gameserver
{
    class GameServer
    {
        public GameState GameState { get; private set; }
        public IStorage Storage { get; private set; }
        private List<User> PlayingUsers = new List<User>();
        object PlayersLock = new object();

        object EventsLock = new object();

        private SyncRecord SyncCity;

        private List<GameEvent> PendingEvents { get; } = new List<GameEvent>();

        private SpawnCity SpawnCity { get; }

        private TimeSpan TickInterval = new TimeSpan(0, 0, 0, 0, 10);


        public GameServer()
        {
            GameState = new GameState();


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
            SpawnCity.SpawnNPCs();

            Thread TickThread = new Thread(TickLoopThread);
            TickThread.Start();
        }

        private void Tick()
        {
            List<GameEvent> sendEvents;

            lock (EventsLock)
            {
                // broken?
                GameState.HandleGameEvents(PendingEvents);
                sendEvents = new List<GameEvent>(PendingEvents);
                var message = new GameEventsMessage(GameState.StepCounter, sendEvents);
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


        public void HandleRequests(IEnumerable<GameEvent> requests)
        {
            PendEvents(requests);
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
                user.PlayerConnection.SendMessage(new GameEventsMessage(0, events));
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

        public void SpawnLootForPlayer(EntityIdentifier looteeId, EntityIdentifier looterId)
        {
            var looter = GameState.GetEntityById(looterId);
            var lootee = GameState.GetEntityById(looteeId);

            if (looter == null || lootee == null || !(looter is PC))
            {
                Logging.Log("Something was null when trying to spawn loot", Logging.LoggingLevel.Error);
                return;
            }

            if (looter is PC)
            {
                // @fix for christ sake just store the PlayerCharacter in the user so we don't end up looping through the entire server every
                // time someone kills something and also this has a very real risk of breaking regardless of what the error message says
                // t ribbe
                User lootingPlayer = null;

                lock (PlayersLock)
                {
                    foreach (var player in PlayingUsers)
                    {
                        if (player.HeroIdentifier.Id == looter.Info.Identifier.Id)
                        {
                            lootingPlayer = player;
                            break;
                        }
                    }
                }

                if (lootingPlayer == null)
                {
                    Logging.Log("this will never happen so you will never see this.", Logging.LoggingLevel.Error);
                    return;
                }

                Item item1 = new Item(ItemType.BronzeDagger, 20, Rarity.Epic,
                    new ItemBonus(ItemBonus.BonusType.Test4, 20),
                    new ItemBonus(ItemBonus.BonusType.Test1, -4)
                    );

                var item2 = new Item(ItemType.Banana, 20, Rarity.Epic);

                var message = new CreateLootMessage(lootee, item1, item2);
                lootingPlayer.PlayerConnection.SendMessage(message);
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

            user.HeroIdentifier = createEvent.Id;
            
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

        private void TickLoopThread()
        {
            var nextTick = DateTime.Now + TickInterval;
            while (true)
            {
                while (DateTime.Now < nextTick)
                {
                    var sleep = nextTick - DateTime.Now;
                    if (sleep.Ticks > 0)
                    {
                        Thread.Sleep(sleep);
                    }

                }
                nextTick += TickInterval; // Notice we're adding onto when the last tick was supposed to be, not when it is now
                                      // Insert tick() code here
                Tick();
            }
        }
    }
}
