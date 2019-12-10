using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.model;
using System.Globalization;
using System.Runtime.InteropServices;

namespace whateverthefuck.src.network.messages
{
    public abstract class WhateverthefuckMessage
    {
#if false
        private const int TypeSize = 1;
        private const int LengthSize = 2;

#endif
        public const int HeaderSize = 4;


        public struct WhateverMessageHeader
        {
            public byte Type { get; set; }
            public ushort Size { get; set; }
        }

        public MessageType MessageType { get; private set; }
        public MessageBody MessageBody { get; set; }

        protected WhateverthefuckMessage(MessageType messageType)
        {
            MessageType = messageType;
        }

        public static WhateverthefuckMessage FromMessageTypex(MessageType type)
        {
            switch (type)
            {
                case MessageType.CreateGameEntityMessage:
                {
                    return new CreateGameEntityMessage();
                }

                case MessageType.ExampleMessage:
                {
                    return new ExampleMessage();
                }

                case MessageType.DeleteGameEntityMessage:
                {
                    return new DeleteGameEntityMessage();
                }

                case MessageType.GrantControlMessage:
                {
                    return new GrantControlMessage();
                }

                case MessageType.LogMessage:
                {
                    return new LogMessage();
                }

                case MessageType.UpdatePlayerControlMessage:
                {
                    return new UpdatePlayerControlMessage();
                }

                case MessageType.UpdateGameStateMessage:
                {
                    return new UpdateGameStateMessage();
                }

                case MessageType.LoginCredentialsMessage:
                {
                    return new LoginCredentialsMessage();
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

        protected virtual byte[] EncodeBody()
        {
            return GetBytes(MessageBody);
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

        protected virtual void DecodeBody(byte[] bs)
        {
            MessageBody body = MessageBody;
            body = (MessageBody)FromBytes(bs, body);
            MessageBody = body;
        }

        public static WhateverMessageHeader ParseHeader(byte[] message)
        {
            WhateverMessageHeader header = new WhateverMessageHeader();
            header = (WhateverMessageHeader)FromBytes(message, header);
            return header;
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

        public override string ToString()
        {
            StringBuilder bodybuild = new StringBuilder();
            var bb = EncodeBody();


            bodybuild.Append(Enum.GetName(typeof(MessageType), MessageType));

            bodybuild.Append("<");
            foreach (var b in bb)
            {
                bodybuild.Append(b.ToString());
                bodybuild.Append(",");
            }
            bodybuild.Append(">");

            return bodybuild.ToString();
        }
    }

    public interface MessageBody
    {
    }



    public struct EntityLocationInfo
    {
        private const char InfoSeperator = '&';

        public int Identifier { get; }
        public float X { get; }
        public float Y { get; }

        public EntityLocationInfo(GameEntity entity) : this(entity.Identifier.Id, entity.Location.X, entity.Location.Y)
        {

        }

        public EntityLocationInfo(int id, float x, float y)
        {
            Identifier = id;
            X = x;
            Y = y;
        }

        // todo should encode to byte[] like a sane person                                                                                                                     
        public string Encode()
        {
            return Identifier.ToString() + InfoSeperator + X.ToString("0.00", CultureInfo.InvariantCulture) + InfoSeperator + Y.ToString("0.00", CultureInfo.InvariantCulture);
        }

        public static EntityLocationInfo Decode(byte[] data)
        {
            string str = System.Text.Encoding.ASCII.GetString(data);

            var dataStrings = str.Split(InfoSeperator);
            int id = Int32.Parse(dataStrings[0]);
            float X = float.Parse(dataStrings[1], CultureInfo.InvariantCulture);
            float Y = float.Parse(dataStrings[2], CultureInfo.InvariantCulture);
            return new EntityLocationInfo(id, X, Y);
        }

    }

    public enum MessageType
    {
        LogMessage,
        UpdateGameStateMessage,
        GrantControlMessage,
        UpdatePlayerControlMessage,
        LoginCredentialsMessage,
        CreateGameEntityMessage,
        DeleteGameEntityMessage,
        ExampleMessage,
    }
}
