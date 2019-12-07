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


        private struct Header
        {
            public byte Type { get; set; }
            public ushort Size { get; set; }
        }

        public MessageType MessageType { get; private set; }


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

                default: throw new NotImplementedException();
            }
        }

        public static byte[] EncodeMessage(WhateverthefuckMessage message)
        {
            byte[] contentBytes = message.EncodeBody();

            Header header = new Header();
            header.Type = (byte)message.MessageType;
            header.Size = (ushort)contentBytes.Length;
            byte[] headerBytes = GetBytes(header);
            return headerBytes.Concat(contentBytes).ToArray();
        }

        protected virtual byte[] EncodeBody()
        {
            return GetBytes(GetBody());
        }

        public static WhateverthefuckMessage DecodeMessage(byte[] message)
        {
            Header header = new Header();
            header = (Header)FromBytes(message, header);
            int headerSize = Marshal.SizeOf(header);


            var messageType = (MessageType)header.Type;
            var bodyLength = header.Size;

            var wmessage = FromMessageTypex(messageType);

            wmessage.DecodeBody(message, headerSize);

            return wmessage;
        }

        protected virtual void DecodeBody(byte[] bs, int arrayOffset)
        {
            MessageBody body = (MessageBody)GetBody();
            body = (MessageBody)FromBytes(bs, body, arrayOffset);
            SetBody(body);
            //return body;
        }

        protected abstract MessageBody GetBody();

        protected abstract void SetBody(MessageBody body);



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
        protected static object FromBytes(IEnumerable<byte> bs, object str, int startIndex = 0)
        {
            byte[] arr = bs.ToArray();

            int size = Marshal.SizeOf(str.GetType());
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, startIndex, ptr, size);

            str = Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
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
        UpdateEntityLocationsMessage,
        GrantControlMessage,
        UpdatePlayerControlMessage,
        LoginCredentialsMessage,
        CreateGameEntityMessage,
        DeleteGameEntityMessage,
        ExampleMessage,
    }
}
