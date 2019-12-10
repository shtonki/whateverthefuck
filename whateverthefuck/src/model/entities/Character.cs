using System;
using System.Drawing;

namespace whateverthefuck.src.model.entities
{
    public abstract class Character : GameEntity
    {
        private const float OneOverSquareRootOfTwo = 0.70710678118f;

        public MovementStruct Movements { get; set; } = new MovementStruct(false, false, false, false);
        public float MoveSpeed = 0.01f;


        public Character(EntityIdentifier identifier, EntityType entityType) : base(identifier, entityType)
        {
            Movable = true;

            HighlightColor = Color.Aqua;
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

        private int UpwardsMask   = 0b0001;
        private int DownwardsMask = 0b0010;
        private int RightwardsMask = 0b0100;
        private int LeftwardsMask  = 0b1000;

        public MovementStruct(bool upwards, bool downwards, bool rightwards, bool leftwards)
        {
            Upwards = upwards;
            Downwards = downwards;
            Rightwards = rightwards;
            Leftwards = leftwards;
        }

        public MovementStruct(int decodeMe)
        {
            Upwards = (UpwardsMask & decodeMe) != 0;
            Downwards = (DownwardsMask & decodeMe) != 0;
            Rightwards = (RightwardsMask & decodeMe) != 0;
            Leftwards = (LeftwardsMask & decodeMe) != 0;
        }

        public int EncodeAsInt()
        {
            int val = 0;

            if (Upwards)
            {
                val |= UpwardsMask;
            }
            if (Downwards)
            {
                val |= DownwardsMask;
            }
            if (Leftwards)
            {
                val |= LeftwardsMask;
            }
            if (Rightwards)
            {
                val |= RightwardsMask;
            }

            return val;
        }
    }
}
