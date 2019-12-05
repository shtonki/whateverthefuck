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
        private GameState GameState;
        private Timer TickTimer;
        private List<WhateverthefuckServerConnection> Players = new List<WhateverthefuckServerConnection>();

        public GameServer()
        {
            GameState = new GameState(true);
            TickTimer = new Timer((_) => Tick(), null, 0, 10);
        }

        public void AddPlayer(WhateverthefuckServerConnection playerConnection)
        {
            Players.Add(playerConnection);
            var pc = SpawnPlayerCharacter();
            pc.SetControl(ControlInfo.ClientControl);
            playerConnection.SendMessage(new GrantControlMessage(pc));
        }

        public void UpdatePlayerCharacterLocation(EntityLocationInfo playerCharacterLocationInfo)
        {
            PlayerCharacter pc = (PlayerCharacter)GameState.GetEntityById(playerCharacterLocationInfo.Identifier);
            pc.Location = new GameCoordinate(playerCharacterLocationInfo.X, playerCharacterLocationInfo.Y);
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
            var es = GameState.AllEntities.Where(e => e.ControlInfo == ControlInfo.ServerControl);
            SendMessageToAllPlayers(new UpdateEntityLocationsMessage(es));
        }
    }
}
