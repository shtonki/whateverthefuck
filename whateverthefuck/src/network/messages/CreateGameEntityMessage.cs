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
        public CreateGameEntityMessage() : base(MessageType.CreateGameEntityMessage)
        {
            MessageBody = new CreateEntityInfo();
        }

        public CreateGameEntityMessage(GameEntity hero) : base(MessageType.CreateGameEntityMessage)
        {
            MessageBody = new CreateEntityInfo(hero);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CreateEntityInfo : MessageBody
    {
        public EntityType EntityType;
        public int Identifier;
        public float X;
        public float Y;

        public CreateEntityInfo(GameEntity entity) : this(
            entity.EntityType,
            entity.Identifier.Id,
            entity.Location.X,
            entity.Location.Y
            )
        {
        }

        public CreateEntityInfo(EntityType entityType, int identifier, float x, float y)
        {
            EntityType = entityType;
            Identifier = identifier;
            X = x;
            Y = y;
        }
    }
}
