﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuckserver.network;
using whateverthefuckserver.storage;

namespace whateverthefuckserver.gameserver
{
    class GamePlayer
    {
        public WhateverthefuckServerConnection PlayerConnection { get; }

        public string Username { get; set; }

        public EntityIdentifier HeroIdentifier { get; set; }

        public Inventory Inventory { get; } = new Inventory();

        public int Experience { get; set; }

        public GamePlayer(WhateverthefuckServerConnection playerConnection, UserInfo info)
        {
            PlayerConnection = playerConnection;

            this.Inventory = info.Inventory;
            this.Username = info.Username;
        }
    }
}