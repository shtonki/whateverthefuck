using System;
using whateverthefuck.src.model;
using whateverthefuck.src.network;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck
{
    class Program
    {
        public static ClientGameStateManager GameStateManager = new ClientGameStateManager();

        public static WhateverClientConnection ServerConnection { get; private set; }

        public static void Main(String[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));
            Logging.Log("Started Logger");

            GUI.CreateGameWindow();

            ConfigFile cf = new ConfigFile("config.json");

            ServerConnection = new WhateverClientConnection();

            UserLogin.Login(cf.ConfigInfo.LoginCredentials);

        }
    }
}
