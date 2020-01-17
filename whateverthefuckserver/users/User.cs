using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuckserver.network;

namespace whateverthefuckserver.users
{
    class User
    {
        public WhateverthefuckServerConnection PlayerConnection { get; }
        public string Username { get; set; }

        public EntityIdentifier HeroIdentifier { get; set; }

        public Inventory Inventory { get; } = new Inventory();

        public User(WhateverthefuckServerConnection playerConnection)
        {
            PlayerConnection = playerConnection;
        }
    }
}
