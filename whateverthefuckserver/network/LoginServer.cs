using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.network;
using whateverthefuck.src.util;
using whateverthefuckserver.users;

namespace whateverthefuckserver.network
{
    static class LoginServer
    {
        private static List<User> LoggedInUsers = new List<User>();

        public static void Login(User user, LoginCredentials loginCredentials)
        {
            user.Username = loginCredentials.Username;
            LoggedInUsers.Add(user);

            Program.GameServer.AddUser(user);
        }

        public static void Logout(WhateverthefuckServerConnection playerConnection)
        {
            var userToLogOut = LoggedInUsers.First(u => u.PlayerConnection == playerConnection);
            if (userToLogOut == null)
            {
                Logging.Log("Tried to log out Connection which was not logged in", Logging.LoggingLevel.Warning);
            }
            LoggedInUsers.Remove(userToLogOut);
            Program.GameServer.RemoveUser(userToLogOut);
        }
    }
}
