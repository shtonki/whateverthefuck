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

        public UpdatePlayerControlBody Body { get; private set; }

        public UpdatePlayerControlMessage() : base(MessageType.UpdatePlayerControlMessage)
        {
            Body = new UpdatePlayerControlBody();
        }

        public UpdatePlayerControlMessage(Character pc) : base(MessageType.UpdatePlayerControlMessage)
        {
            Body = new UpdatePlayerControlBody(pc);
        }


        protected override MessageBody GetBody()
        {
            return Body;
        }

        protected override void SetBody(MessageBody body)
        {
            Body = (UpdatePlayerControlBody)body;
        }
    }

    public struct UpdatePlayerControlBody : MessageBody
    {
        public int EntityId { get; }
        //public MovementStruct MovementStruct { get; }
        public bool Upwards { get; }
        public bool Downwards { get; }
        public bool Leftwards { get; }
        public bool Rightwards { get; }

        public MovementStruct MovementStruct => new MovementStruct(Upwards, Downwards, Rightwards, Leftwards);

        public UpdatePlayerControlBody(Character entity) : this()
        {
            EntityId = entity.Identifier.Id;
            Upwards = entity.Movements.Upwards;
            Downwards = entity.Movements.Downwards;
            Leftwards = entity.Movements.Leftwards;
            Rightwards = entity.Movements.Rightwards;
        }
    }
}
