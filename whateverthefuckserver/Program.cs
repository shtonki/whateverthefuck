using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;
using whateverthefuckserver.network;

namespace whateverthefuckserver
{
    class Program
    {
        private static GameServer GameServer;

        public static WhateverServer ServerConnection;

        public static void Main(string[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));

            ServerConnection = new WhateverServer();
            ServerConnection.StartListening();

            GameServer = new GameServer();
        }
    }
}
