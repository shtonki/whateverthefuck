using System;
using System.Collections.Generic;
using System.Drawing;
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


        public static void Main(String[] args)
        {
#if false
            List<GameEvent> ges = new List<GameEvent>();
            ges.Add(new DummyEvent(420));
            ges.Add(new DummyEvent(69));
            ges.Add(new Dummy2Event(4.19f, 111));
            ges.Add(new Dummy2Event(6.9f, 222));
            ges.Add(new DummyEvent(5));
            ges.Add(new MoveEntityEvent(4, 1.4f, 1.6f));


            UpdateGameStateMessage message = new UpdateGameStateMessage(ges);
            var bs = WhateverthefuckMessage.EncodeMessage(message);

            var reconstructedMessage = WhateverthefuckMessage.DecodeMessage(bs);
            int i = 4;
#else
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));

            Logging.Log("Running version: " + WhateverthefuckVersion.CurrentVersion.ToString());

            GUI.CreateGameWindow();
            Logging.Log("Created Game Window", Logging.LoggingLevel.Info);

            GUI.LoadGUI();
            Logging.Log("Loaded GUI Components", Logging.LoggingLevel.Info);

            UserSettings.LoadUserSettings();

            ServerConnection = new WhateverClientConnection();
            Logging.Log("Connected to Server", Logging.LoggingLevel.Info);

            UserLogin.Login(UserSettings.Config.Username);
            Logging.Log(String.Format("Logged on to Server as {0}", UserSettings.Config.Username), Logging.LoggingLevel.Info);
#endif
            }
    }
}
