using whateverthefuck.src.model;
using whateverthefuck.src.util;
using System.Threading;
using whateverthefuckserver.network;
using System.Collections.Generic;
using whateverthefuck.src.network.messages;
using System.Linq;
using whateverthefuck.src.model.entities;
using System;

namespace whateverthefuckserver
{
    class GameServer
    {
        public GameState GameState { get; private set; }
        private Timer TickTimer;
        private List<WhateverthefuckServerConnection> Players = new List<WhateverthefuckServerConnection>();

        public GameServer()
        {
            GameState = new GameState();
            TickTimer = new Timer((_) => Tick(), null, 0, 10);
        }

        public void AddPlayer(WhateverthefuckServerConnection playerConnection)
        {
            Players.Add(playerConnection);
            var pc = SpawnPlayerCharacter();
            playerConnection.SendMessage(new GrantControlMessage(pc));
        }

        public void RemovePlayer(WhateverthefuckServerConnection playerConnection)
        {
            Players.Remove(playerConnection);
        }

        public void UpdatePlayerCharacterLocation(int id, MovementStruct movementStruct)
        {
            PlayerCharacter pc = (PlayerCharacter)GameState.GetEntityById(id);
            pc.Movements = movementStruct;
        }

        private PlayerCharacter SpawnPlayerCharacter()
        {
            var pc = GameState.EntityGenerator.GeneratePlayerCharacter(new GameCoordinate(0, 0), false);
            GameState.AddEntity(pc);
            SendMessageToAllPlayers(new AddPlayerCharacterMessage(pc));

            return pc;
        }

        private void SendMessageToAllPlayers(WhateverthefuckMessage message)
        {
            // todo we encode the message seperately for each client

            foreach (var client in Players)
            {
                client.SendMessage(message);
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
