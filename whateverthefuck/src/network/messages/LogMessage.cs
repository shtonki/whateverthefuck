
namespace whateverthefuck.src.network.messages
{
    public class LogMessage : WhateverthefuckMessage
    {
        public string Message { get; }

        public LogMessage(byte[] body) : base(MessageType.LogMessage)
        {
            Message = System.Text.Encoding.ASCII.GetString(body);
        }

        public LogMessage(string message) : base(MessageType.LogMessage)
        {
            Message = message;
        }

        protected override byte[] EncodeBody()
        {
            return System.Text.Encoding.ASCII.GetBytes(Message);
        }
    }
}
