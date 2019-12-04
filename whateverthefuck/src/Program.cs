using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.network;
using whateverthefuck.src.model;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck
{
    class Program
    {
        public static GameState GameState = new GameState();

        public static void Main(String[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));
            Logging.Log("Started Logger");
            GUI.CreateGameWindow();

            WhateverClientConnection c = new WhateverClientConnection();
        }
    }
}
