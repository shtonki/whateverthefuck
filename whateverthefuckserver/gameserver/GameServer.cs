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

namespace whateverthefuckserver
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

            var house = GameState.EntityGenerator.GenerateHouse(5, 5);
            GameState.HandleGameEvents(house.Select(b => new CreateEntityEvent(b)));
        }

        internal void HandleEventRequests(List<GameEvent> events)
        {
            PendEvents(events.ToArray());
        }

        public void AddUser(User user)
        {
            var createEvents = GameState.AllEntities.Select(e => new CreateEntityEvent(e));
            user.PlayerConnection.SendMessage(new UpdateGameStateMessage(createEvents));

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
            GameEvent re = new DestroyEntityEvent(hero);
            PendEvents(re);
        }

        private void SpawnPlayerCharacter(User user)
        {
            var pc = GameState.EntityGenerator.GenerateEntity(EntityType.PlayerCharacter);
            pc.Location = new GameCoordinate(0, 0);
            user.HeroIdentifier = pc.Identifier;
            
            // create entity on serverside game state
            var cee = new CreateEntityEvent(pc);
            PendEvents(cee);

            // send message to user granting control to created character
            user.PlayerConnection.SendMessage(new GrantControlMessage((PlayerCharacter)pc));
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

        private void PendEvents(params GameEvent[] es)
        {
            lock (EventsLock)
            {
                PendingEvents.AddRange(es);
            }
        }

        public void Tick()
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
