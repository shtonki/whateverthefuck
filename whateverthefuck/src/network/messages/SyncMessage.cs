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

        public SyncMessage(int tick, long checksum) : base(MessageType.SyncMessage)
        {
            MessageBody = new SyncMessageBody(tick, checksum);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SyncMessageBody : MessageBody
    {
        public int Tick;
        public long Checksum;

        public SyncMessageBody(int tick, long checksum)
        {
            Tick = tick;
            Checksum = checksum;
        }
    }
}
