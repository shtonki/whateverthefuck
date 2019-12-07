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
            TickTimer = new Timer((_) => Tick(), null, 0, 1);

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

            NPC mob = (NPC)GameState.EntityGenerator.GenerateEntity(EntityType.NPC);
            GameState.AddEntity(mob);

            Block block = (Block)GameState.EntityGenerator.GenerateEntity(EntityType.Block);
            block.Location = new GameCoordinate(0.6f, 0.0f);
            GameState.AddEntity(block);

            block = (Block)GameState.EntityGenerator.GenerateEntity(EntityType.Block);
            block.Location = new GameCoordinate(0.6f, 0.9f);
            GameState.AddEntity(block);
#if true
            for (int i = 0; i < 10; i++)
            {
                 block = (Block)GameState.EntityGenerator.GenerateEntity(EntityType.Block);
                block.Location = new GameCoordinate(0.5f, i*0.1f);
                GameState.AddEntity(block);
            }
#endif
        }

        public void AddUser(User user)
        {
            foreach (var entity in GameState.AllEntities)
            {
                user.PlayerConnection.SendMessage(new CreateGameEntityMessage(entity));
            }

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
            var hero = GameState.GetEntityById(user.HeroIdentifier.Id);
            GameState.RemoveEntity(hero);
            SendMessageToAllPlayers(new DeleteGameEntityMessage(hero));
        }

        public void UpdatePlayerCharacterMovementStruct(int id, MovementStruct movementStruct)
        {
            PlayerCharacter pc = (PlayerCharacter)GameState.GetEntityById(id);
            pc.Movements = movementStruct;
        }

        private void SpawnPlayerCharacter(User user)
        {
            var pc = GameState.EntityGenerator.GenerateEntity(EntityType.PlayerCharacter);
            pc.Location = new GameCoordinate(0, 0);
            GameState.AddEntity(pc);
            user.HeroIdentifier = pc.Identifier;
            SendMessageToAllPlayers(new CreateGameEntityMessage(pc));
            user.PlayerConnection.SendMessage(new GrantControlMessage((PlayerCharacter)pc));
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
