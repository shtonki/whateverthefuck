using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.network.messages
{
    class ExampleMessage : WhateverthefuckMessage
    {
        public ExampleMessage() : base(MessageType.ExampleMessage)
        {
            MessageBody = new ExampleBody(0, 0);
        }

        public ExampleMessage(int a, float b) : base(MessageType.ExampleMessage)
        {
            MessageBody = new ExampleBody(a, b);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ExampleBody : MessageBody
    {
        public int a;
        public float b;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string bs;

        public ExampleBody(int a, float b)
        {
            this.a = a;
            this.b = b;
            bs = "walla";
        }
    }
}
