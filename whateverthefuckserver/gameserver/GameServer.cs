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

        internal void HandleEventRequests(List<GameEvent> events)
        {
            PendEvents(events);
        }

        public void AddUser(User user)
        {
            IEnumerable<GameEvent> createEvents = GameState.AllEntities.Select(e => new CreateEntityEvent(e));
            IEnumerable<GameEvent> movementEvents = GameState.AllEntities.Where(e => e.Movements.IsMoving).Select(e => new UpdateMovementEvent(e));


            var events = createEvents.Concat(movementEvents).ToArray();

            user.PlayerConnection.SendMessage(new UpdateGameStateMessage(events));

            lock (PlayersLock)
            {
                PlayingUsers.Add(user);
            }
            SpawnPlayerCharacter(user);
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

        private void Tick()
        {
            UpdateGameStateMessage message;

            lock (EventsLock)
            {
                GameState.HandleGameEvents(PendingEvents);
                message = new UpdateGameStateMessage(PendingEvents);
                PendingEvents.Clear();
            }

            GameState.Step();
            SendMessageToAllPlayers(message);
        }
    }
}
