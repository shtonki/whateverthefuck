using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;
using whateverthefuck.src.view;
using whateverthefuckserver.network;

namespace whateverthefuckserver
{
    class Program
    {
        public static GameServer GameServer { get; private set; }

        public static WhateverConnectionListeningServer ServerConnection;

        public static void Main(string[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));

            ServerConnection = new WhateverConnectionListeningServer();
            ServerConnection.StartListening();
            Logging.Log("Started Listening for Connections", Logging.LoggingLevel.Info);

            GameServer = new GameServer();
            Logging.Log("Started Game Server", Logging.LoggingLevel.Info);

            if (false)
            {
                GUI.CreateGameWindow();
                GUI.ForceToDrawGameState = GameServer.GameState;
                Logging.Log("Created Game Window", Logging.LoggingLevel.Info);
            }
        }
    }
}
