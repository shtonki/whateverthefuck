using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;
using whateverthefuck.src.view;
using whateverthefuckserver.gameserver;
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

            Logging.Log("Running version: " + WhateverthefuckVersion.CurrentVersion.ToString());

            ServerConnection = new WhateverConnectionListeningServer();
            ServerConnection.StartListening();
            Logging.Log("Started Listening for Connections", Logging.LoggingLevel.Info);

            GameServer = new GameServer();
            Logging.Log("Started Game Server", Logging.LoggingLevel.Info);

            if (false)
            {
#pragma warning disable CS0162 // Unreachable code detected
                GUI.CreateGameWindow();
                GUI.ForceToDrawGameState = GameServer.GameState;
                Logging.Log("Created Game Window", Logging.LoggingLevel.Info);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }
    }
}
