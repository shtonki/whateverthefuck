using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network
{
    public class UserLogin
    {
        public static void Login(LoginCredentials loginCreds)
        {
            Program.ServerConnection.SendMessage(new SendLoginCredentialsMessage(loginCreds));
        }
    }
}
