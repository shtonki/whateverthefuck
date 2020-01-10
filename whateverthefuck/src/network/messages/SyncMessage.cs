namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;
    using whateverthefuck.src.util;

    public class SyncMessage : WhateverthefuckMessage
    {
        public SyncMessage()
            : base(MessageType.SyncMessage)
        {
        }

        public SyncMessage(SyncRecord record)
            : base(MessageType.SyncMessage)
        {
            this.SyncRecord = record;
        }

        public SyncRecord SyncRecord { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            this.SyncRecord.Encode(encoder);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            var record = new SyncRecord();
            record.Decode(decoder);
            this.SyncRecord = record;
        }
    }

    public struct SyncRecord : IEncodable
    {
        public SyncRecord(int tick, long hash)
        {
            this.Tick = tick;
            this.Hash = hash;
        }

        public int Tick { get; private set; }

        public long Hash { get; private set; }

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.Tick);
            encoder.Encode(this.Hash);
        }

        public void Decode(WhateverDecoder decoder)
        {
            this.Tick = decoder.DecodeInt();
            this.Hash = decoder.DecodeLong();
        }
    }
}
