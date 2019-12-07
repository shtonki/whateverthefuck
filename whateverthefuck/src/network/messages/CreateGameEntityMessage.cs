using System;
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

        protected override MessageBody GetBody()
        {
            return CreateEntityInfo;
        }

        protected override void SetBody(MessageBody body)
        {
            CreateEntityInfo = (CreateEntityInfo)body;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateEntityInfo : MessageBody
    {
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
    }
}
