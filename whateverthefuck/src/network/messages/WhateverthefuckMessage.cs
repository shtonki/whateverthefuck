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
    public class WhateverthefuckMessage
    {
        private const int TypeSize = 1;
        private const int LengthSize = 2;
        public const int HeaderSize = TypeSize + LengthSize;

        private struct Header
        {
            public byte Type { get; set; }
            public ushort Size { get; set; }
        }

        public MessageType MessageType { get; private set; }


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

                default: throw new NotImplementedException();
            }
        }

        public byte[] Encode()
        {
            byte[] contentBytes = GetBytes(GetBodyx());

            Header header = new Header();
            header.Type = (byte)MessageType;
            header.Size = (ushort)contentBytes.Length;
            byte[] headerBytes = GetBytes(header);
            //byte[] bytes = new byte[headerBytes.Length + contentBytes.Length];
            return headerBytes.Concat(contentBytes).ToArray();
        }

        public static WhateverthefuckMessage Decode(byte[] message)
        {
            Header header = new Header();
            header = (Header)FromBytes(message, header);
            int headerSize = Marshal.SizeOf(header);


            var messageType = (MessageType)header.Type;
            var bodyLength = header.Size;

            var wmessage = FromMessageTypex(messageType);

            MessageBody body = (MessageBody)wmessage.GetBodyx();
            body = (MessageBody)FromBytes(message, body, headerSize);
            wmessage.SetBodyx(body);

            return wmessage;
        }


        public byte[] EncodeO()
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

        public static WhateverthefuckMessage DecodeO(MessageType type, byte[] body)
        {
            switch (type)
            {
                case MessageType.Log:
                    {
                        return new LogMessage(body);
                    }

                case MessageType.UpdateEntityLocations:
                    {
                        return new UpdateEntityLocationsMessage(body);
                    }

                case MessageType.CreateGameEntityMessage:
                    {
                        //return new CreateGameEntityMessage(body);
                        return null;
                    }

                case MessageType.GrantControlMessage:
                    {
                        return new GrantControlMessage(body);
                    }

                case MessageType.UpdatePlayerControlMessage:
                    {
                        return new UpdatePlayerControlMessage(body);
                    }

                case MessageType.LoginCredentialsMessage:
                    {
                        return new SendLoginCredentialsMessage(body);
                    }

                case MessageType.DeleteGameEntityMessage:
                    {
                        return new DeleteGameEntityMessage(body);
                    }

                default:
                    {
                        throw new Exception("received wonky message header");
                    }
            }
        }

        protected virtual byte[] EncodeBody()
        {
            throw new NotImplementedException();
        }

        protected virtual object EncodeBodyx()
        {
            throw new NotImplementedException();
        }

        protected virtual MessageBody GetBodyx()
        {
            if (MessageType == MessageType.CreateGameEntityMessage)
            {
                CreateGameEntityMessage thisAsCGEM = (CreateGameEntityMessage)this;
            }

            throw new NotImplementedException();
        }

        protected virtual void SetBodyx(MessageBody body)
        {
            throw new NotImplementedException();
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
        Log,
        UpdateEntityLocations,
        GrantControlMessage,
        UpdatePlayerControlMessage,
        LoginCredentialsMessage,
        CreateGameEntityMessage,
        DeleteGameEntityMessage,
        ExampleMessage,
    }
}
