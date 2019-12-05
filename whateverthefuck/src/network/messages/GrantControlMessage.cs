using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.network.messages
{
    public class GrantControlMessage : WhateverthefuckMessage
    {
        public int Id { get; }

        public GrantControlMessage(PlayerCharacter pc) : base(MessageType.GrantControlMessage)
        {
            Id = pc.Identifier.Id;
        }

        public GrantControlMessage(byte[] body) : base(MessageType.GrantControlMessage)
        {
            Id = (body[0] << 24) |
                 (body[1] << 16) |
                 (body[2] << 8)  |
                 (body[3]);
        }

        protected override byte[] EncodeBody()
        {
            byte[] bytes = new byte[4];

            bytes[0] = (byte)(Id >> 24);
            bytes[1] = (byte)(Id >> 16);
            bytes[2] = (byte)(Id >> 8);
            bytes[3] = (byte)Id;

            return bytes;
        }
    }
}
