using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class UpdatePlayerControlMessage : WhateverthefuckMessage
    {
        public UpdatePlayerControlMessage() : base(MessageType.UpdatePlayerControlMessage)
        {
            MessageBody = new UpdatePlayerControlBody();
        }

        public UpdatePlayerControlMessage(Character pc) : base(MessageType.UpdatePlayerControlMessage)
        {
            MessageBody = new UpdatePlayerControlBody(pc);
        }
    }

    public struct UpdatePlayerControlBody : MessageBody
    {
        public int EntityId { get; }
        public MovementStruct MovementStruct { get; }

        public UpdatePlayerControlBody(Character entity) : this()
        {
            EntityId = entity.Identifier.Id;
            MovementStruct = entity.Movements;
        }
    }
}
