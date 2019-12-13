namespace whateverthefuck.src.network.messages
{
    using System.Runtime.InteropServices;

    public class LogMessage : WhateverthefuckMessage
    {
        public LogMessage()
            : base(MessageType.LogMessage)
        {
            this.MessageBody = new LogBody();
        }

        public LogMessage(string message)
            : base(MessageType.LogMessage)
        {
            this.MessageBody = new LogBody(message);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LogBody : IMessageBody
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Message;

        public LogBody(string message)
        {
            this.Message = message;
        }
    }
}
