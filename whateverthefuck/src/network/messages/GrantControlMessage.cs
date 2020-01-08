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
            Id = new EntityIdentifier(id);
        }

        public GrantControlMessage(byte[] bs)
            : base(MessageType.GrantControlMessage)
        {
            WhateverDecoder decoder = new WhateverDecoder(bs);

            this.Id = new EntityIdentifier(decoder.DecodeInt());
        }

        public EntityIdentifier Id { get; }

        protected override byte[] EncodeBody()
        {
            WhateverEncoder encoder = new WhateverEncoder();

            encoder.Encode(this.Id.Id);

            return encoder.GetBytes();
        }
    }
}
