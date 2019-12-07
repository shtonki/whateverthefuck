using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class DeleteGameEntityMessage : WhateverthefuckMessage
    {
        public DeleteGameEntityBody Body { get; private set; }

        public DeleteGameEntityMessage() : base(MessageType.DeleteGameEntityMessage)
        {
            Body = new DeleteGameEntityBody();
        }

        public DeleteGameEntityMessage(GameEntity entity) : base(MessageType.DeleteGameEntityMessage)
        {
            Body = new DeleteGameEntityBody(entity);
        }

        protected override MessageBody GetBody()
        {
            return Body;
        }

        protected override void SetBody(MessageBody body)
        {
            Body = (DeleteGameEntityBody)body;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DeleteGameEntityBody : MessageBody
    {
        public int Id;

        public DeleteGameEntityBody(GameEntity entity)
        {
            Id = entity.Identifier.Id;
        }
    }
}
