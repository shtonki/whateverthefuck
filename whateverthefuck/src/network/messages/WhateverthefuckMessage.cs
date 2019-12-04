using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;
using whateverthefuck.network.messages;

namespace whateverthefuck.network.messages
{
    public abstract class WhateverthefuckMessage
    {
        private const int TypeSize = 1;
        private const int LengthSize = 2;
        public const int HeaderSize = TypeSize + LengthSize;
        

        protected MessageType MessageType { get; }

        protected WhateverthefuckMessage(MessageType messageType)
        {
            MessageType = messageType;
        }

        public byte[] Encode()
        {
            var body = EncodeBody();
            byte typeByte = (byte)MessageType;
            if (body.Length > Int16.MaxValue) { Logging.Log("we have big message and big problem.", Logging.LoggingLevel.Error); }
            Int16 lengthBytes = (short)body.Length;
            byte lengthLowEndian = (byte)(lengthBytes & 0xFF);
            byte lengthHighEndian = (byte)(lengthBytes >> 8);

            byte[] message = new byte[HeaderSize + body.Length];
            message[0] = typeByte;
            message[1] = lengthLowEndian;
            message[2] = lengthHighEndian;
            Array.Copy(body, 0, message, 3, body.Length);

            return message;
        }

        public static WhateverthefuckMessage Decode(MessageType type, byte[] body)
        {
            switch (type)
            {
                case MessageType.LogMessage:
                {
                    return new LogMessage(body);
                } 

                default:
                {
                    throw new Exception("received wonky message");
                }
            }
        }

        public abstract byte[] EncodeBody();
    }

    public enum MessageType
    {
        LogMessage,
    }
}
