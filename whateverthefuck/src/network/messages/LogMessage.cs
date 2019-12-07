
using System.Runtime.InteropServices;

namespace whateverthefuck.src.network.messages
{
    public class LogMessage : WhateverthefuckMessage
    {
        public LogMessage() : base(MessageType.LogMessage)
        {
            MessageBody = new LogBody();
        }

        public LogMessage(string message) : base(MessageType.LogMessage)
        {
            MessageBody = new LogBody(message);
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
