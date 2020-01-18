using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.network;
using whateverthefuck.src.util;
using whateverthefuckserver.gameserver;
using whateverthefuckserver.storage;

namespace whateverthefuckserver.network
{
    static class LoginServer
    {
        private static List<GamePlayer> LoggedInUsers = new List<GamePlayer>();

        public static GamePlayer Login(WhateverthefuckServerConnection connection, LoginCredentials loginCredentials)
        {
            var username = loginCredentials.Username;

            var info = SebasLocalDatabase.Instance.GetUserInfo(username);

            var user = new GamePlayer(connection, info);

            LoggedInUsers.Add(user);
            Program.GameServer.AddUser(user);

            return user;
        }

        public static void Logout(WhateverthefuckServerConnection playerConnection)
        {
            var userToLogOut = LoggedInUsers.First(u => u.PlayerConnection == playerConnection);
            if (userToLogOut == null)
            {
                Logging.Log("Tried to log out Connection which was not logged in", Logging.LoggingLevel.Warning);
            }
            LoggedInUsers.Remove(userToLogOut);
            Program.GameServer.LogoutUser(userToLogOut);
        }
    }
}
