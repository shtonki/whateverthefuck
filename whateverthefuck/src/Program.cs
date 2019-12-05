using System;
using whateverthefuck.src.model;
using whateverthefuck.src.network;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck
{
    class Program
    {
        public static GameState GameState = new GameState(false);

        public static void Main(String[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));
            Logging.Log("Started Logger");
            GUI.CreateGameWindow();

            WhateverClientConnection c = new WhateverClientConnection();
        }
    }
}
