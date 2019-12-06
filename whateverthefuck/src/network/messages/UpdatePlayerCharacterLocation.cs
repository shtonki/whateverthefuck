using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.network.messages
{
    public class UpdatePlayerControlMessage : WhateverthefuckMessage
    {
        public int EntityId { get; }
        public MovementStruct MovementStruct { get; }

        public UpdatePlayerControlMessage(PlayerCharacter pc) : base(MessageType.UpdatePlayerControlMessage)
        {
            MovementStruct = pc.Movements;
            EntityId = pc.Identifier.Id;
        }

        public UpdatePlayerControlMessage(byte[] body) : base(MessageType.UpdatePlayerControlMessage)
        {
            EntityId = DecodeInt(body.Take(sizeof(int)).ToArray());
            MovementStruct = MovementStruct.Decode(body.Skip(sizeof(int)).ToArray());
        }

        protected override byte[] EncodeBody()
        {
            var movementArray = MovementStruct.Encode();
            var encoded = new byte[sizeof(int) + movementArray.Length];
            Array.Copy(EncodeInt(EntityId), 0, encoded, 0, sizeof(int));
            Array.Copy(movementArray, 0, encoded, sizeof(int), movementArray.Length);

            return encoded;
        }
    }
}
