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
        private ExampleBody Body = new ExampleBody(4, 20.0f);

        public ExampleMessage() : base(MessageType.ExampleMessage)
        {
        }

        protected override MessageBody GetBodyx()
        {
            return Body;
        }

        protected override void SetBodyx(MessageBody body)
        {
            Body = (ExampleBody)body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ExampleBody : MessageBody
    {
        int a;
        float b;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        string bs;

        public ExampleBody(int a, float b)
        {
            this.a = a;
            this.b = b;
            bs = "walla";
        }
    }
}
