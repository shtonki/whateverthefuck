using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class GrantControlMessage : WhateverthefuckMessage
    {
        public GrantControlMessage() : base(MessageType.GrantControlMessage)
        {
            MessageBody = new GrantControlBody();
        }

        public GrantControlMessage(GameEntity entity) : base(MessageType.GrantControlMessage)
        {
            MessageBody = new GrantControlBody(entity);
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GrantControlBody : MessageBody
    {
        public int Id;

        public GrantControlBody(GameEntity entity)
        {
            Id = entity.Identifier.Id;
        }
    }
}
