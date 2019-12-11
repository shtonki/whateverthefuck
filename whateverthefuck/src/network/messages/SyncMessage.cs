using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.network.messages
{
    public class SyncMessage : WhateverthefuckMessage
    {
        public SyncMessage() : base(MessageType.SyncMessage)
        {
            MessageBody = new SyncMessageBody();
        }

        public SyncMessage(SyncMessageBody body) : base(MessageType.SyncMessage)
        {
            MessageBody = body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SyncMessageBody : MessageBody
    {
        public int Tick;
        public long Hash;

        public SyncMessageBody(int tick, long checksum)
        {
            Tick = tick;
            Hash = checksum;
        }
    }
}
