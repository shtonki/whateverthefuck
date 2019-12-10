using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public abstract class Character : GameEntity
    {

        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed = 0.01f;




        public Character(EntityIdentifier identifier, EntityType entityType) : base(identifier, entityType)
        {
            Movable = true;

            HighlightColor = Color.Aqua;
        }
#if false
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

#endif

        public override GameCoordinate CalculateMovement()
        {
            if (float.IsNaN(Movements.Direction))
            {
                return new GameCoordinate(0, 0);
            }
            else
            {
                return new GameCoordinate((float)Math.Sin(Movements.Direction) * MoveSpeed, (float)Math.Cos(Movements.Direction) * MoveSpeed);
            }
#if false
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
#endif
        }
    }

    public class MovementStruct
    {
        public float Direction { get; set; }
        public int? FollowId { get; set; }

        public static MovementStruct Decode(byte[] bs)
        {
            IEnumerable<byte> bytes = bs;
            bool isFollow = BitConverter.ToBoolean(bytes.ToArray(), 0);
            bytes = bytes.Skip(sizeof(bool));

            var ms = new MovementStruct();

            if (isFollow)
            {
                ms.FollowId = BitConverter.ToInt32(bytes.ToArray(), 0);
            }
            else
            {
                ms.Direction = BitConverter.ToSingle(bytes.ToArray(), 0);
            }

            return ms;

        }
        public byte[] Encode()
        {
            
            if (FollowId.HasValue)
            {
                return BitConverter.GetBytes(true).Concat(BitConverter.GetBytes(FollowId.Value)).ToArray();
            }
            else
            {
                return BitConverter.GetBytes(false).Concat(BitConverter.GetBytes(Direction)).ToArray();
            }
        }
    }

}
