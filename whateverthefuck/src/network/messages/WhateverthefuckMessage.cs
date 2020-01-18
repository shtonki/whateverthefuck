namespace whateverthefuck.src.network.messages
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using whateverthefuck.src.model;
    using whateverthefuck.src.util;

    public struct WhateverMessageHeader : IEncodable
    {
        public const int HeaderSize = sizeof(int) + sizeof(int);

        public WhateverMessageHeader(MessageType type, int size)
        {
            this.MessageType = type;
            this.MessageSize = (ushort)size;
        }

        public MessageType MessageType { get; private set; }

        public int MessageSize { get; private set; }

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.MessageType);
            encoder.Encode(this.MessageSize);
        }

        public void Decode(WhateverDecoder decoder)
        {
            this.MessageType = (MessageType)decoder.DecodeInt();
            this.MessageSize = decoder.DecodeInt();
        }
    }

    public abstract class WhateverthefuckMessage : IEncodable
    {
        protected WhateverthefuckMessage(MessageType messageType)
        {
            this.MessageType = messageType;
        }

        public MessageType MessageType { get; private set; }

        public abstract void Encode(WhateverEncoder encoder);

        public abstract void Decode(WhateverDecoder decoder);

        public static byte[] EncodeMessage(WhateverthefuckMessage message)
        {
            WhateverEncoder encoder = new WhateverEncoder();

            message.Encode(encoder);

            return encoder.GetBytes();
        }

        public static WhateverthefuckMessage DecodeMessage(MessageType type, byte[] bs)
        {
            WhateverDecoder decoder = new WhateverDecoder(bs);

            var wmessage = FromMessageType(type);
            wmessage.Decode(decoder);

            return wmessage;
        }

        public static WhateverthefuckMessage FromMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.GameEventMessage:
                {
                    return new GameEventsMessage();
                }

                case MessageType.ExampleMessage:
                {
                    return new ExampleMessage();
                }

                case MessageType.CreateLootMessage:
                {
                    return new CreateLootMessage();
                }

                case MessageType.LoginMessage:
                {
                    return new LoginMessage();
                }

                case MessageType.SyncMessage:
                {
                    return new SyncMessage();
                }

                case MessageType.AddItemToInventoryMessage:
                {
                    return new AddItemToInventoryMessage();
                }

                case MessageType.GrantControlMessage:
                {
                    return new GrantControlMessage();
                }

                default: throw new NotImplementedException();
            }
        }
    }

    public enum MessageType
    {
        ExampleMessage,

        LoginMessage,

        SyncMessage,

        CreateGameStateMessage,

        GrantControlMessage,
        CreateLootMessage,

        GameEventMessage,

        UseItemMessage,
        AddItemToInventoryMessage,
    }
}
