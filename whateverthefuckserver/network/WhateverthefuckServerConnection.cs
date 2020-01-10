using System;
using System.Net.Sockets;
using whateverthefuck.src.network;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuckserver.users;

namespace whateverthefuckserver.network
{
    class WhateverthefuckServerConnection : WhateverthefuckConnection
    {

        public WhateverthefuckServerConnection(NetworkStream stream) : base(stream)
        {

        }

        protected override void HandleConnectionDeath()
        {
            LoginServer.Logout(this);
        }

        protected override void HandleMessage(WhateverthefuckMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.LoginMessage:
                {
                    LoginMessage loginMessage = (LoginMessage)message;
                    LoginServer.Login(new User(this), new LoginCredentials(loginMessage.LoginCredentials.Username));         
                } break;

                case MessageType.SyncMessage:
                {
                    SyncMessage syncMessage = (SyncMessage)message;
                    Program.GameServer.InSync(syncMessage.SyncRecord.Tick, syncMessage.SyncRecord.Hash);
                } break;

                default:
                {
                    Logging.Log("Unhandled message of type " + message.MessageType, Logging.LoggingLevel.Warning);
                } break;
            }
        }
    }
}
