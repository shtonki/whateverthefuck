using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
                case MessageType.UpdatePlayerControlMessage:
                {
                    UpdatePlayerControlBody updatePlayerChatacterLocationBody = (UpdatePlayerControlBody)message.MessageBody;
                    Program.GameServer.UpdatePlayerCharacterMovementStruct(updatePlayerChatacterLocationBody.EntityId, updatePlayerChatacterLocationBody.MovementStruct);
                } break;

                case MessageType.LoginCredentialsMessage:
                {
                    LoginCredentialBody loginBody = (LoginCredentialBody) message.MessageBody;
                    LoginServer.Login(new User(this), new LoginCredentials(loginBody.Username));         
                } break;
                default: throw new NotImplementedException();
            }
        }
    }
}
