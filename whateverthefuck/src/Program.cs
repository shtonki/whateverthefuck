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
            Loot lz = new Loot(new EntityIdentifier(13), CreationArgs.Zero);
            Loot lnz = new Loot(new EntityIdentifier(21), CreationArgs.Zero);

            Item zeros = new Item(ItemType.Test1, 0, Rarity.Common);
            Item notzeros = new Item(ItemType.Test7, 420, Rarity.Rare,
                new ItemBonus(ItemBonus.BonusType.Test1), new ItemBonus(ItemBonus.BonusType.Test2),
                new ItemBonus(ItemBonus.BonusType.Test1), new ItemBonus(ItemBonus.BonusType.Test2)
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
            Logging.Log(String.Format("Logged on to Server as {0}", UserSettings.Config.Username), Logging.LoggingLevel.Info);
#endif
            }
    }
}
