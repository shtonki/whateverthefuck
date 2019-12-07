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
        public DeleteGameEntityMessage() : base(MessageType.DeleteGameEntityMessage)
        {
            MessageBody = new DeleteGameEntityBody();
        }

        public DeleteGameEntityMessage(GameEntity entity) : base(MessageType.DeleteGameEntityMessage)
        {
            MessageBody = new DeleteGameEntityBody(entity);
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
