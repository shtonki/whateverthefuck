using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class DeleteGameEntityMessage : WhateverthefuckMessage
    {
        public EntityIdentifier Identifier { get; }

        public DeleteGameEntityMessage(GameEntity entity) : base(MessageType.DeleteGameEntityMessage)
        {
            Identifier = entity.Identifier;
        }

        public DeleteGameEntityMessage(byte[] body) : base(MessageType.DeleteGameEntityMessage)
        {
            Identifier = new EntityIdentifier(WhateverEncoding.DecodeInt(body));
        }

        protected override byte[] EncodeBody()
        {
            return WhateverEncoding.EncodeInt(Identifier.Id);
            throw new NotImplementedException();
        }
    }
}
