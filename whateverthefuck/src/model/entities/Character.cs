﻿using System;

namespace whateverthefuck.src.model.entities
{
    public abstract class Character : GameEntity
    {
        private const float OneOverSquareRootOfTwo = 0.70710678118f;

        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed = 0.01f;


        public Character(EntityIdentifier identifier, EntityType entityType) : base(identifier, entityType)
        {
            Movable = true;
        }

        public void SetMovementUpwards(bool move)
        {
            Movements.Upwards = move;
        }

        public void SetMovementDownwards(bool move)
        {
            Movements.Downwards = move;
        }
        
        public void SetMovementLeftwards(bool move)
        {
            Movements.Leftwards = move;
        }

        public void SetMovementRightwards(bool move)
        {
            Movements.Rightwards = move;
        }

        public override GameCoordinate CalculateMovement()
        {
            var xMove = 0.0f;
            var yMove = 0.0f;

            var speed = MoveSpeed;

            if ((Movements.Upwards ^ Movements.Downwards) & (Movements.Leftwards ^ Movements.Rightwards))
            {
                speed = speed * OneOverSquareRootOfTwo;
            }

            if (Movements.Upwards) { yMove += speed; }
            if (Movements.Downwards) { yMove -= speed; }
            if (Movements.Leftwards) { xMove -= speed; }
            if (Movements.Rightwards) { xMove += speed; }

            return new GameCoordinate(xMove, yMove);
        }
    }

    public class MovementStruct
    {
        public bool Upwards { get; set; }
        public bool Downwards { get; set; }
        public bool Rightwards { get; set; }
        public bool Leftwards { get; set; }

    }
}
