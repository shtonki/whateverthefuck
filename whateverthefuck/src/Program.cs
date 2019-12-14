namespace whateverthefuck
{
    using System;
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
#if false
            ItemBonus b1 = new ItemBonus(0x00040014);
            ItemBonus b2 = new ItemBonus(ItemBonus.BonusType.Test4, 20);

            Loot lz = new Loot(new EntityIdentifier(13), CreationArgs.Zero);
            Loot lnz = new Loot(new EntityIdentifier(21), CreationArgs.Zero);

            Item zeros = new Item(ItemType.Test1, 0, Rarity.Common);
            Item notzeros = new Item(ItemType.Test7, 420, Rarity.Rare,
                new ItemBonus(ItemBonus.BonusType.Test1, 4), new ItemBonus(ItemBonus.BonusType.Test2, 3),
                new ItemBonus(ItemBonus.BonusType.Test1, 5), new ItemBonus(ItemBonus.BonusType.Test2, 6)
                );

            CreateLootMessage m1 = new CreateLootMessage(lnz, notzeros);
            var bs1 = WhateverthefuckMessage.EncodeMessage(m1);

            CreateLootMessage m2 = new CreateLootMessage(lz, zeros);
            var bs2 = WhateverthefuckMessage.EncodeMessage(m2);

            var reconMessage1 = (CreateLootMessage)WhateverthefuckMessage.DecodeMessage(bs1);
            var reconMessage2 = (CreateLootMessage)WhateverthefuckMessage.DecodeMessage(bs2);

            Item zerosBack = reconMessage2.Item;
            Item nonzerosBack = reconMessage1.Item;

            int v = 4;
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
            Logging.Log(string.Format("Logged on to Server as {0}", UserSettings.Config.Username), Logging.LoggingLevel.Info);

#endif
        }
    }
}
