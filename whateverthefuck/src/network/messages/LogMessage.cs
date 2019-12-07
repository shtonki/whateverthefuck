
using System.Runtime.InteropServices;

namespace whateverthefuck.src.network.messages
{
    public class LogMessage : WhateverthefuckMessage
    {
        public LogBody Body { get; private set; }

        public LogMessage() : base(MessageType.LogMessage)
        {
        }

        public LogMessage(string message) : base(MessageType.LogMessage)
        {
            Body = new LogBody(message);
        }


        protected override MessageBody GetBody()
        {
            return Body;
        }

        protected override void SetBody(MessageBody body)
        {
            Body = (LogBody)body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LogBody : MessageBody
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Message;

        public LogBody(string message)
        {
            Message = message;
        }
    }
}
