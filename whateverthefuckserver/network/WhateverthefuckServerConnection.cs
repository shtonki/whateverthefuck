using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.network;
using whateverthefuck.src.network.messages;

namespace whateverthefuckserver.network
{
    class WhateverthefuckServerConnection : WhateverthefuckConnection
    {

        public WhateverthefuckServerConnection(NetworkStream stream) : base(stream)
        {

        }

        protected override void HandleMessage(WhateverthefuckMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.UpdatePlayerControlMessage:
                {
                    UpdatePlayerControlMessage updatePlayerCharacterLocationMessage = (UpdatePlayerControlMessage)message;
                    Program.GameServer.UpdatePlayerCharacterLocation(updatePlayerCharacterLocationMessage.EntityId, updatePlayerCharacterLocationMessage.MovementStruct);
                } break;

                default: throw new NotImplementedException();
            }
        }
    }
}
