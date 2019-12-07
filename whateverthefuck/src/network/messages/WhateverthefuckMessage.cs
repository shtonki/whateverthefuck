﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.model;
using System.Globalization;

namespace whateverthefuck.src.network.messages
{
    public abstract class WhateverthefuckMessage
    {
        private const int TypeSize = 1;
        private const int LengthSize = 2;
        public const int HeaderSize = TypeSize + LengthSize;


        public MessageType MessageType { get; }

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
                        return new CreateGameEntityMessage(body);
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

        protected abstract byte[] EncodeBody();

    }


    public class EntityLocationInfo
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
    }
}