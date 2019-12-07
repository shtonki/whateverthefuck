using System;
using System.IO;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.network;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck
{
    class Program
    {
        public static ClientGameStateManager GameStateManager = new ClientGameStateManager();

        public static WhateverClientConnection ServerConnection { get; private set; }

        struct WriteMe
        {
            public int Val { get; set; }
            public string Str { get; set; }

            public WriteMe(int val, string str)
            {
                Val = val;
                Str = str;
            }
        }

        public static void Main(String[] args)
        {
#if true
            int length;

            NPC npc = new NPC(new EntityIdentifier(4));
            CreateGameEntityMessage message = new CreateGameEntityMessage(npc);
            byte[] bs = message.EncodeBodyButPublic();

            CreateGameEntityMessage reconstructedMessage = new CreateGameEntityMessage(bs);

            Console.WriteLine(reconstructedMessage.CreateEntityInfo.EntityType);
            Console.WriteLine(reconstructedMessage.CreateEntityInfo.LocationInfo.Identifier);
#else

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
#endif
        }
    }
}
