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

        private User User { get; set; }

        protected override void HandleConnectionDeath()
        {
            LoginServer.Logout(this);
        }

        protected override void HandleMessage(WhateverthefuckMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.GameEventMessage:
                {
                    GameEventsMessage gem = (GameEventsMessage)message;
                    Program.GameServer.HandleRequests(this.User, gem.Events);
                } break;

                case MessageType.LoginMessage:
                {
                    LoginMessage loginMessage = (LoginMessage)message;
                    var user = new User(this);
                    if (LoginServer.Login(user, new LoginCredentials(loginMessage.LoginCredentials.Username)))
                    {
                        this.User = user;
                    };         
                } break;

                case MessageType.SyncMessage:
                {
                    SyncMessage syncMessage = (SyncMessage)message;
                    Program.GameServer.InSync(syncMessage.SyncRecord.Tick, syncMessage.SyncRecord.Hash);
                } break;

                case MessageType.AddItemToInventoryMessage:
                {
                    AddItemToInventoryMessage aitim = (AddItemToInventoryMessage)message;
                    User.Inventory.AddItem(aitim.Item);
                } break;

                default:
                {
                    Logging.Log("Unhandled message of type " + message.MessageType, Logging.LoggingLevel.Warning);
                } break;
            }
        }
    }
}
