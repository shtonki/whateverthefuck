﻿
using System.Runtime.InteropServices;

namespace whateverthefuck.src.network.messages
{
    public class LogMessage : WhateverthefuckMessage
    {
        public LogBody Body { get; private set; }

        public LogMessage() : base(MessageType.LogMessage)
        {
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
        public string Username;

        public LogBody(string username)
        {
            Username = username;
        }
    }
}
