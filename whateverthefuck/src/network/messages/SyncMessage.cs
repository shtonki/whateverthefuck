namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;
    using whateverthefuck.src.util;

    public class SyncMessage : WhateverthefuckMessage
    {
        public SyncMessage(byte[] bs)
            : base(MessageType.SyncMessage)
        {
            WhateverDecoder decoder = new WhateverDecoder(bs);

            this.Tick = decoder.DecodeInt();
            this.Hash = decoder.DecodeLong();
        }

        public SyncMessage(int tick, long hash)
            : base(MessageType.SyncMessage)
        {
            this.Tick = tick;
            this.Hash = hash;
        }

        public int Tick { get; }

        public long Hash { get; }

        protected override byte[] EncodeBody()
        {
            WhateverEncoder encoder = new WhateverEncoder();

            encoder.Encode(this.Tick);
            encoder.Encode(this.Hash);

            return encoder.GetBytes();
        }
    }

}
