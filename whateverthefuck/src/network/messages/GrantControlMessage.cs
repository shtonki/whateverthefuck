﻿using System;
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
            Id = DecodeInt(body);
        }

        protected override byte[] EncodeBody()
        {
            return EncodeInt(Id);
        }
    }
}
