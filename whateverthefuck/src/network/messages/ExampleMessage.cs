namespace whateverthefuck.src.network.messages
{
    using System;
    using System.Runtime.InteropServices;
    using whateverthefuck.src.util;

    internal class ExampleMessage : WhateverthefuckMessage
    {
        public ExampleMessage(int a, float b, string c)
            : base(MessageType.ExampleMessage)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public ExampleMessage(byte[] body)
            : base(MessageType.ExampleMessage)
        {
            WhateverDecoder decoder = new WhateverDecoder(body);

            this.A = decoder.DecodeInt();
            this.B = decoder.DecodeFloat();
            this.C = decoder.DecodeString();
        }

        public int A { get; private set; }

        public float B { get; private set; }

        public string C { get; private set; }

        protected override byte[] EncodeBody()
        {
            WhateverEncoder encoder = new WhateverEncoder();

            encoder.Encode(this.A);
            encoder.Encode(this.B);
            encoder.Encode(this.C);

            return encoder.GetBytes();
        }
    }
}
