﻿using System;
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
        public string Username { get; }

        public EntityIdentifier HeroIdentifier { get; set; }

        public User(WhateverthefuckServerConnection playerConnection, string username)
        {
            PlayerConnection = playerConnection;
            Username = username;
        }
    }
}
