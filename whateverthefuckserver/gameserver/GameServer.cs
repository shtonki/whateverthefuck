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
using System.Diagnostics;

namespace whateverthefuckserver.gameserver
{
    class GameServer
    {
        public GameState GameState { get; private set; }
        public IStorage Storage { get; private set; }
        private List<GamePlayer> PlayingUsers = new List<GamePlayer>();
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


        public void HandleRequests(GamePlayer user, IEnumerable<GameEvent> requests)
        {
            if (CheckRequests(user, requests))
            {
                PendEvents(requests);
            }
        }

        public void AddUser(GamePlayer player)
        {
            var message = GenerateCreateGameStateMessage(player);

            lock (PlayersLock)
            {
                player.PlayerConnection.SendMessage(message);
                PlayingUsers.Add(player);
            }

            Logging.Log("Added player");
        }

        public void LogoutUser(GamePlayer user)
        {
            lock (PlayersLock)
            {
                PlayingUsers.Remove(user);
            }
            var hero = GameState.GetEntityById(user.HeroIdentifier.Id);

            SebasLocalDatabase.Instance.StoreUserInfo(new UserInfo(user.Username, user.Inventory, hero.Equipment));

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
                GamePlayer lootingPlayer = null;

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

                var message = new CreateLootMessage(
                    lootee,
                    new Banana(Rarity.Common, 20));
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

        public void RequestTransaction(TransactionMessage transactionMessage, GamePlayer player)
        {
            var transaction = transactionMessage.Transaction;

            if (!transaction.CanAfford(player.Inventory))
            {
                return;
            }

            player.PlayerConnection.SendMessage(transactionMessage);


            transaction.Execute(player.Inventory);
        }

        private bool CheckRequests(GamePlayer requester, IEnumerable<GameEvent> requests)
        {
            foreach (var request in requests)
            {
                switch (request.Type)
                {
                    case GameEventType.UpdateMovement:
                    {
                        var ume = (UpdateMovementEvent)request;
                        if (ume.Identifier.Id == requester.HeroIdentifier.Id)
                        {
                            return true;
                        }
                    } break;

                    case GameEventType.BeginCastAbility:
                    {
                        var bcae = (BeginCastAbilityEvent)request;
                        if (bcae.CasterIdentifier.Id == requester.HeroIdentifier.Id)
                        {
                            return true;
                        }
                    } break;

                    case GameEventType.UseItem:
                    {
                        var uie = (UseItemEvent)request;

                        var item = requester.Inventory.GetIdentical(uie.Item);

                        if (item != null)
                        {
                            if (item.DepletesOnUse)
                            {
                                item.StackSize--;
                            }

                            return true;
                        }
                    } break;

                    case GameEventType.EquipItem:
                    {
                        var eie = (EquipItemEvent)request;

                        var item = requester.Inventory.GetIdentical(eie.Item);

                        if (item != null && eie.Equipper.Id == requester.HeroIdentifier.Id)
                        {
                            return true;
                        }
                    } break;
                }
            }

            return false;
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

        private CompoundMessage GenerateCreateGameStateMessage(GamePlayer player)
        {
            var messages = new List<WhateverthefuckMessage>();

            // make sure we add items to inventory before we try to equip them
            messages.Add(new AddItemsToInventoryMessage(player.Inventory.AllItems));

            List<GameEvent> events = new List<GameEvent>();

            // spawn the hero first
            var heroIdentifier = SpawnCity.SpawnHero();

            var equipEvents = player.EquippedToHero.Equipped.Select(item => new EquipItemEvent(heroIdentifier, item)).ToArray();
            PendEvents(equipEvents);

            player.HeroIdentifier = heroIdentifier;

            // send message to user granting control to created character


            // @dubious do this after every tick and cache it instead
            lock (EventsLock)
            {
                foreach (var entity in GameState.AllEntities)
                {
                    // @incomplete (if entity wasn't generated by map) generate CreateEntityEvent
                    events.Add(new CreateEntityEvent(entity));
                    // if entity moves generate UpdateMovementEvent
                    if (entity.Movements.IsMoving) { events.Add(new UpdateMovementEvent(entity)); }
                    // if entity has stati applied to it generate AppluStatusEvents
                    if (entity.Status?.ActiveStatuses.Count > 0) 
                        foreach (var status in entity.Status.ActiveStatuses) { events.Add(new ApplyStatusEvent(entity, status)); }
                }
            }



            messages.Add(new GameEventsMessage(0, events));

            messages.Add(new GrantControlMessage(heroIdentifier));

            return new CompoundMessage(messages);
        }
    }
}
