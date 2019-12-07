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
            WhateverthefuckMessage example = new ExampleMessage(4 , 4.20f);
            var bs = WhateverthefuckMessage.EncodeMessage(example);


            var rebuiltMessage = WhateverthefuckMessage.DecodeMessage(bs);
            int i = 5;
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
