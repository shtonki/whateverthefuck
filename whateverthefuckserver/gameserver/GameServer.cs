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
                catch (Exception e)
                {
                }
            }).Start();
        }

        public void AddUser(User user)
        {
            PlayingUsers.Add(user);
            SpawnUserAsPlayerCharacter(user);
        }

        public void SpawnUserAsPlayerCharacter(User user)
        {
            SpawnPlayerCharacter(user);
        }

        public void RemoveUser(User user)
        {
            PlayingUsers.Remove(user);
            GameState.RemoveEntity(user.HeroIdentifier);
        }

        public void UpdatePlayerCharacterLocation(int id, MovementStruct movementStruct)
        {
            PlayerCharacter pc = (PlayerCharacter)GameState.GetEntityById(id);
            pc.Movements = movementStruct;
        }

        private void SpawnPlayerCharacter(User user)
        {
            var pc = GameState.EntityGenerator.GeneratePlayerCharacter(new GameCoordinate(0, 0), false);
            GameState.AddEntity(pc);
            user.HeroIdentifier = pc.Identifier;
            SendMessageToAllPlayers(new AddPlayerCharacterMessage(pc));
            user.PlayerConnection.SendMessage(new GrantControlMessage(pc));
        }

        private void SendMessageToAllPlayers(WhateverthefuckMessage message)
        {
            // todo we encode the message seperately for each client

            foreach (var user in PlayingUsers)
            {
                user.PlayerConnection.SendMessage(message);
            }
        }

        public void Tick()
        {
            GameState.Step();

            var es = GameState.AllEntities.Where(e => e.Movable);
            SendMessageToAllPlayers(new UpdateEntityLocationsMessage(es));
        }
    }
}
