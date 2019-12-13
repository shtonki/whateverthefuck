namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;

    internal class ExampleMessage : WhateverthefuckMessage
    {
        public ExampleMessage()
            : base(MessageType.ExampleMessage)
        {
            this.MessageBody = new ExampleBody(0, 0);
        }

        public ExampleMessage(int a, float b)
            : base(MessageType.ExampleMessage)
        {
            this.MessageBody = new ExampleBody(a, b);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ExampleBody : IMessageBody
    {
        public int A;
        public float B;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string Bs;

        public ExampleBody(int a, float b)
        {
            this.A = a;
            this.B = b;
            this.Bs = "walla";
        }
    }
}
