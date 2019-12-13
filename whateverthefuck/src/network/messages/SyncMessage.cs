namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;

    public class SyncMessage : WhateverthefuckMessage
    {
        public SyncMessage()
            : base(MessageType.SyncMessage)
        {
            this.MessageBody = new SyncMessageBody();
        }

        public SyncMessage(SyncMessageBody body)
            : base(MessageType.SyncMessage)
        {
            this.MessageBody = body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SyncMessageBody : IMessageBody
    {
        public int Tick;
        public long Hash;

        public SyncMessageBody(int tick, long checksum)
        {
            this.Tick = tick;
            this.Hash = checksum;
        }
    }
}
