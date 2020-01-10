using System;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class GrantControlMessage : WhateverthefuckMessage
    {
        public GrantControlMessage(EntityIdentifier controlledIdentifier)
            : base(MessageType.GrantControlMessage)
        {
            this.ControlledIdentifier = controlledIdentifier;
        }

        public EntityIdentifier ControlledIdentifier { get; private set; }

        public override void Decode(WhateverDecoder decoder)
        {
            throw new NotImplementedException();
        }

        public override void Encode(WhateverEncoder encoder)
        {
            throw new NotImplementedException();
        }
    }
}
