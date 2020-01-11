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

                case MessageType.GrantControlMessage:
                {
                    return new GrantControlMessage();
                }

                default: throw new NotImplementedException();
            }
        }
#if false
        public static byte[] EncodeMessage(WhateverthefuckMessage message)
        {
            byte[] contentBytes = message.EncodeBody();

            WhateverMessageHeader header = new WhateverMessageHeader();
            header.Type = (byte)message.MessageType;
            header.Size = (ushort)contentBytes.Length;
            byte[] headerBytes = GetBytes(header);
            return headerBytes.Concat(contentBytes).ToArray();
        }

        public static WhateverthefuckMessage DecodeMessage(byte[] bs)
        {
            WhateverMessageHeader header = ParseHeader(bs);

            int headerSize = Marshal.SizeOf(header);

            var messageType = (MessageType)header.Type;
            var bodyLength = header.Size;

            var wmessage = FromMessageTypex(messageType);

            wmessage.DecodeBody(bs.Skip(headerSize).ToArray());

            return wmessage;
        }

        public static WhateverthefuckMessage DecodeMessage(WhateverMessageHeader header, byte[] body)
        {
            int headerSize = Marshal.SizeOf(header);

            var messageType = (MessageType)header.Type;
            var bodyLength = header.Size;

            var wmessage = FromMessageTypex(messageType);

            wmessage.DecodeBody(body);

            return wmessage;
        }

        public static WhateverMessageHeader ParseHeader(byte[] message)
        {
            WhateverMessageHeader header = new WhateverMessageHeader();
            header = (WhateverMessageHeader)FromBytes(message, header);
            return header;
        }

        public override string ToString()
        {
            StringBuilder bodybuild = new StringBuilder();
            var bb = this.EncodeBody();

            bodybuild.Append(Enum.GetName(typeof(MessageType), this.MessageType));

            bodybuild.Append("<");
            foreach (var b in bb)
            {
                bodybuild.Append(b.ToString());
                bodybuild.Append(",");
            }

            bodybuild.Append(">");

            return bodybuild.ToString();
        }

        protected static byte[] GetBytes(object str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        protected static object FromBytes(IEnumerable<byte> bs, object str)
        {
            byte[] arr = bs.ToArray();

            int size = Marshal.SizeOf(str.GetType());
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            var typ = str.GetType();
            str = Marshal.PtrToStructure(ptr, typ);
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        protected virtual byte[] EncodeBody()
        {
            return GetBytes(this.MessageBody);
        }

        protected virtual void DecodeBody(byte[] bs)
        {
            IMessageBody body = this.MessageBody;
            body = (IMessageBody)FromBytes(bs, body);
            this.MessageBody = body;
        }

        public struct WhateverMessageHeader
        {
            public byte Type { get; set; }

            public ushort Size { get; set; }
        }
#endif
    }

    public enum MessageType
    {
        ExampleMessage,

        LoginMessage,

        SyncMessage,

        GrantControlMessage,
        CreateLootMessage,

        GameEventMessage,
    }

#if true
    public struct EntityLocationInfo
    {
        private const char InfoSeperator = '&';

        public EntityLocationInfo(GameEntity entity)
            : this(entity.Identifier.Id, entity.GameLocation.X, entity.GameLocation.Y)
        {
        }

        public EntityLocationInfo(int id, float x, float y)
        {
            this.Identifier = id;
            this.X = x;
            this.Y = y;
        }

        public int Identifier { get; }

        public float X { get; }

        public float Y { get; }

        public static EntityLocationInfo Decode(byte[] data)
        {
            string str = System.Text.Encoding.ASCII.GetString(data);

            var dataStrings = str.Split(InfoSeperator);
            int id = int.Parse(dataStrings[0]);
            float x = float.Parse(dataStrings[1], CultureInfo.InvariantCulture);
            float y = float.Parse(dataStrings[2], CultureInfo.InvariantCulture);
            return new EntityLocationInfo(id, x, y);
        }

        // todo should encode to byte[] like a sane person
        public string Encode()
        {
            return this.Identifier.ToString() + InfoSeperator + this.X.ToString("0.00", CultureInfo.InvariantCulture) + InfoSeperator + this.Y.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
#endif
}
