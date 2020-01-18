using System;
using System.Net.Sockets;
using whateverthefuck.src.network;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuckserver.gameserver;

namespace whateverthefuckserver.network
{
    class WhateverthefuckServerConnection : WhateverthefuckConnection
    {

        public WhateverthefuckServerConnection(NetworkStream stream) : base(stream)
        {

        }

        public GamePlayer User { get; private set; }

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
                    this.User = LoginServer.Login(this, new LoginCredentials(loginMessage.LoginCredentials.Username));
                } break;

                case MessageType.SyncMessage:
                {
                    SyncMessage syncMessage = (SyncMessage)message;
                    Program.GameServer.InSync(syncMessage.SyncRecord.Tick, syncMessage.SyncRecord.Hash);
                } break;

                case MessageType.AddItemsToInventoryMessage:
                {
                    AddItemsToInventoryMessage aitim = (AddItemsToInventoryMessage)message;
                    User.Inventory.AddItems(aitim.Items);
                } break;

                default:
                {
                    Logging.Log("Unhandled message of type " + message.MessageType, Logging.LoggingLevel.Warning);
                } break;
            }
        }
    }
}
