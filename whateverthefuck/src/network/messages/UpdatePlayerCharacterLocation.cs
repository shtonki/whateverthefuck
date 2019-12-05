using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.network.messages
{
    public class UpdatePlayerCharacterLocationMessage : WhateverthefuckMessage
    {
        public EntityLocationInfo PlayerCharacterLocationInfo { get; }

        public UpdatePlayerCharacterLocationMessage(PlayerCharacter pc) : base(MessageType.UpdatePlayerCharacterLocation)
        {
            PlayerCharacterLocationInfo = new EntityLocationInfo(pc);
        }

        public UpdatePlayerCharacterLocationMessage(byte[] body) : base(MessageType.UpdatePlayerCharacterLocation)
        {
            PlayerCharacterLocationInfo = EntityLocationInfo.Decode(body);
        }

        protected override byte[] EncodeBody()
        {
            return System.Text.Encoding.ASCII.GetBytes(PlayerCharacterLocationInfo.Encode());
        }
    }
}
