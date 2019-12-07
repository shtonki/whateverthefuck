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
        public GrantControlBody Body { get; private set; }

        public GrantControlMessage(GameEntity entity) : base(MessageType.DeleteGameEntityMessage)
        {
            Body = new GrantControlBody(entity);
        }

        protected override MessageBody GetBody()
        {
            return Body;
        }

        protected override void SetBody(MessageBody body)
        {
            Body = (GrantControlBody)body;
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
