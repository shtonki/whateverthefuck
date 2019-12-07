﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.network.messages
{
    public class CreateGameEntityMessage : WhateverthefuckMessage
    {
        public CreateEntityInfo CreateEntityInfo { get; private set; }

        public CreateGameEntityMessage() : base(MessageType.CreateGameEntityMessage)
        {
            CreateEntityInfo = new CreateEntityInfo();
        }

        public CreateGameEntityMessage(GameEntity hero) : base(MessageType.CreateGameEntityMessage)
        {
            CreateEntityInfo = new CreateEntityInfo(hero);
        }

        protected override byte[] EncodeBody()
        {
            return GetBytes(CreateEntityInfo);
        }

        protected override object EncodeBodyx()
        {
            return CreateEntityInfo;
        }

        protected override MessageBody GetBodyx()
        {
            return CreateEntityInfo;
        }

        protected override void SetBodyx(MessageBody body)
        {
            CreateEntityInfo = (CreateEntityInfo)body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateEntityInfo : MessageBody
    {
#if true
        public EntityType EntityType;
        public int Identifier;
        public float X;
        public float Y;
        public float Z;
        public float xd;

        public CreateEntityInfo(GameEntity entity) : this(
            entity.EntityType,
            entity.Identifier.Id,
            entity.Location.X,
            entity.Location.Y, 
            4f,
            20f
            )
        {
        }

        public CreateEntityInfo(EntityType entityType, int identifier, float x, float y, float z, float xd)
        {
            EntityType = entityType;
            Identifier = identifier;
            X = x;
            Y = y;
            Z = z;
            this.xd = xd;
        }
#else
        public EntityType EntityType { get; }

        public EntityLocationInfo LocationInfo { get; }

        public CreateEntityInfo(GameEntity entity)
        {
            LocationInfo = new EntityLocationInfo(entity);
            EntityType = entity.EntityType;
        }

        public CreateEntityInfo(EntityLocationInfo locationInfo, EntityType entityType)
        {
            LocationInfo = locationInfo;
            EntityType = entityType;
        }

        public static CreateEntityInfo Decode(byte[] bytes)
        {
            byte[] decodeBuffer;

            decodeBuffer = new byte[sizeof(int)];
            Array.Copy(bytes, 0, decodeBuffer, 0, sizeof(int));
            var entityType = (EntityType)(WhateverEncoding.DecodeInt(decodeBuffer));

            decodeBuffer = new byte[bytes.Length - sizeof(int)];
            Array.Copy(bytes, sizeof(int), decodeBuffer, 0, bytes.Length - sizeof(int));

            return new CreateEntityInfo(EntityLocationInfo.Decode(decodeBuffer), entityType);
        }

        public byte[] Encode()
        {
            var type = WhateverEncoding.EncodeInt((int)EntityType);
            var locationInfo = System.Text.Encoding.ASCII.GetBytes(LocationInfo.Encode());
            var bytes = new byte[locationInfo.Length + type.Length];
            Array.Copy(type, 0, bytes, 0, type.Length);
            Array.Copy(locationInfo, 0, bytes, type.Length, locationInfo.Length);
            return bytes;
        }
#endif
    }
}
