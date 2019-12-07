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

            Logging.Log("Running version: " + WhateverthefuckVersion.CurrentVersion.ToString());

            GUI.CreateGameWindow();
            Logging.Log("Created Game Window", Logging.LoggingLevel.Info);

            ConfigFile cf = new ConfigFile("config.json");
            Logging.Log("Loaded Config File", Logging.LoggingLevel.Info);

            ServerConnection = new WhateverClientConnection();
            Logging.Log("Connected to Server", Logging.LoggingLevel.Info);

            UserLogin.Login(cf.ConfigInfo.LoginCredentials);
            Logging.Log(String.Format("Logged on to Server as {0}", cf.ConfigInfo.LoginCredentials.Username), Logging.LoggingLevel.Info);

        }
    }
}
