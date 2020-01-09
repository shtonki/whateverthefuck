using System;
using System.Net.Sockets;
using whateverthefuck.src.network;
using whateverthefuck.src.network.messages;
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
                case MessageType.LoginCredentialsMessage:
                {
                    LoginMessage loginMessage = (LoginMessage)message;
                    LoginServer.Login(new User(this), new LoginCredentials(loginMessage.LoginCredentials.Username));         
                } break;

                case MessageType.UpdateGameStateMessage:
                {
                        UpdateGameStateMessage updateMessage = (UpdateGameStateMessage)message;
                        Program.GameServer.HandleEventRequests(updateMessage.Events);
                } break;

                case MessageType.SyncMessage:
                {
                    SyncMessage syncMessage = (SyncMessage)message;
                    Program.GameServer.InSync(syncMessage.SyncRecord.Tick, syncMessage.SyncRecord.Hash);
                } break;

                default: throw new NotImplementedException("Unhandled MessageType");
            }
        }
    }
}
