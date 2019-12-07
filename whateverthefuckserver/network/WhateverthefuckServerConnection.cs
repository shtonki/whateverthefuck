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
                    UpdatePlayerControlMessage updatePlayerCharacterLocationMessage = (UpdatePlayerControlMessage)message;
                    Program.GameServer.UpdatePlayerCharacterLocation(updatePlayerCharacterLocationMessage.Body.EntityId, updatePlayerCharacterLocationMessage.Body.MovementStruct);
                } break;

                case MessageType.LoginCredentialsMessage:
                {
                    LoginCredentialsMessage credentialsMessage = (LoginCredentialsMessage)message;
                    LoginServer.Login(new User(this), new LoginCredentials(credentialsMessage.Body.Username));         
                } break;
                default: throw new NotImplementedException();
            }
        }
    }
}
