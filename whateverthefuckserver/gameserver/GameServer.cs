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

        public void AddUser(User user)
        {
            var createEvents = GameState.AllEntities.Select(e => new CreateEntityEvent(e));
            user.PlayerConnection.SendMessage(new UpdateGameStateMessage(createEvents));

            lock (PlayersLock)
            {
                PlayingUsers.Add(user);
            }
            SpawnUserAsPlayerCharacter(user);

        }

        public void SpawnUserAsPlayerCharacter(User user)
        {
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
            GameState.HandleGameEvents(re);
            SendMessageToAllPlayers(new UpdateGameStateMessage(re));
        }

        public void UpdatePlayerCharacterMovementStruct(int id, MovementStruct movementStruct)
        {
            PlayerCharacter pc = (PlayerCharacter)GameState.GetEntityById(id);
            var evnt = new UpdateControlEvent(id, movementStruct);
            PendingEvents.Add(evnt);
        }

        private void SpawnPlayerCharacter(User user)
        {
            var pc = GameState.EntityGenerator.GenerateEntity(EntityType.PlayerCharacter);
            pc.Location = new GameCoordinate(0, 0);
            //GameState.AddEntities(pc);
            user.HeroIdentifier = pc.Identifier;
            var cee = new CreateEntityEvent(pc);
            GameState.HandleGameEvents(cee);
            SendMessageToAllPlayers(new UpdateGameStateMessage(cee));
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

        public void Tick()
        {
            GameState.HandleGameEvents(PendingEvents);
            var message = new UpdateGameStateMessage(PendingEvents);
            PendingEvents.Clear();
            GameState.Step();
            SendMessageToAllPlayers(message);
        }
    }
}
