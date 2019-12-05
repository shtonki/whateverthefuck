using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.network.messages
{
    public class AddPlayerCharacterMessage : WhateverthefuckMessage
    {
        public EntityLocationInfo HeroInfo { get; }

        public AddPlayerCharacterMessage(byte[] body) : base(MessageType.AddPlayerCharacterMessage)
        {
            HeroInfo = EntityLocationInfo.Decode(body);
        }

        public AddPlayerCharacterMessage(PlayerCharacter hero) : base(MessageType.AddPlayerCharacterMessage)
        {
            HeroInfo = new EntityLocationInfo(hero);
        }

        protected override byte[] EncodeBody()
        {
            return System.Text.Encoding.ASCII.GetBytes(HeroInfo.Encode());
        }
    }
}
