namespace whateverthefuck
{
    using System;
    using System.Threading;
    using whateverthefuck.src.model;
    using whateverthefuck.src.network;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    internal class Program
    {
        public static ClientGameStateManager GameStateManager { get; } = new ClientGameStateManager();

        public static WhateverClientConnection ServerConnection { get; private set; }

        public static void Main(string[] args)
        {
            Logging.AddLoggingOutput(new ConsoleOutput(Logging.LoggingLevel.All, true));
            Logging.SetDefaultLoggingLevel(Logging.LoggingLevel.Info);

#if false
            Game.TextorMain();
#else

            Logging.Log("Running version: " + WhateverthefuckVersion.CurrentVersion.ToString());

            GUI.CreateGameWindow();
            Logging.Log("Created Game Window", Logging.LoggingLevel.Info);

            GUI.LoadGUI();
            Logging.Log("Loaded GUI Components", Logging.LoggingLevel.Info);

            UserSettings.LoadUserSettings();

            ContextHandler.SetupUnifiedContext();

            BoomBoxSetterUpper.SetupBoombox();

            ServerConnection = new WhateverClientConnection();
            Logging.Log("Connected to Server", Logging.LoggingLevel.Info);

            UserLogin.Login(UserSettings.Config.Username);
            Logging.Log(string.Format("Logged on to Server as {0}", UserSettings.Config.Username), Logging.LoggingLevel.Info);
#endif
        }
    }
}
