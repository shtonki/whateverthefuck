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

        public static void Main(string[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));

            GameServer = new GameServer();

            WhateverServerConnection wsc = new WhateverServerConnection();
            wsc.StartListening();
        }
    }
}
