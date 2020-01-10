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

        public GrantControlMessage()
            : base(MessageType.GrantControlMessage)
        {
        }

        public EntityIdentifier ControlledIdentifier { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.ControlledIdentifier.Id);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.ControlledIdentifier = new EntityIdentifier(decoder.DecodeInt());
        }
    }
}
