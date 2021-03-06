﻿namespace whateverthefuck.src.network.messages
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using whateverthefuck.src.model;

    public abstract class WhateverthefuckMessage
    {
#if false
        private const int TypeSize = 1;
        private const int LengthSize = 2;

#endif
        public const int HeaderSize = 4;

        protected WhateverthefuckMessage(MessageType messageType)
        {
            this.MessageType = messageType;
        }

        public MessageType MessageType { get; private set; }

        public IMessageBody MessageBody { get; set; }

        public static WhateverthefuckMessage FromMessageTypex(MessageType type)
        {
            switch (type)
            {
                case MessageType.ExampleMessage:
                {
                    return new ExampleMessage();
                }

                case MessageType.GrantControlMessage:
                {
                    return new GrantControlMessage();
                }

                case MessageType.LogMessage:
                {
                    return new LogMessage();
                }

                case MessageType.UpdateGameStateMessage:
                {
                    return new UpdateGameStateMessage();
                }

                case MessageType.LoginCredentialsMessage:
                {
                    return new LoginCredentialsMessage();
                }

                case MessageType.SyncMessage:
                {
                    return new SyncMessage();
                }

                case MessageType.CreateLootMessage:
                {
                    return new CreateLootMessage();
                }

                default: throw new NotImplementedException();
            }
        }

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
    }

    public enum MessageType
    {
        ExampleMessage,

        LogMessage,

        UpdateGameStateMessage,
        GrantControlMessage,
        LoginCredentialsMessage,
        SyncMessage,

        CreateLootMessage,
    }

    public interface IMessageBody
    {
    }

    public struct EntityLocationInfo
    {
        private const char InfoSeperator = '&';

        public EntityLocationInfo(GameEntity entity)
            : this(entity.Identifier.Id, entity.Location.X, entity.Location.Y)
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
}
