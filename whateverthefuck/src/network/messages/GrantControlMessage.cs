namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;
    using whateverthefuck.src.model;
    using whateverthefuck.src.util;

    public class GrantControlMessage : WhateverthefuckMessage
    {
        public GrantControlMessage(int id)
            : base(MessageType.GrantControlMessage)
        {
            this.Id = new EntityIdentifier(id);
        }

        public GrantControlMessage()
            : base(MessageType.GrantControlMessage)
        {
        }

        public EntityIdentifier Id { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.Id.Id);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Id = new EntityIdentifier(decoder.DecodeInt());
        }
    }
}
