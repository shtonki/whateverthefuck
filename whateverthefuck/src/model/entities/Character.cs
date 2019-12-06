using System;

namespace whateverthefuck.src.model.entities
{
    public abstract class Character : GameEntity
    {
        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed = 0.01f;


        public Character(EntityIdentifier identifier) : base(identifier)
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

            if (Movements.Upwards) { yMove += MoveSpeed; }
            if (Movements.Downwards) { yMove -= MoveSpeed; }
            if (Movements.Leftwards) { xMove -= MoveSpeed; }
            if (Movements.Rightwards) { xMove += MoveSpeed; }

            return new GameCoordinate(xMove, yMove);
        }
    }

    public class MovementStruct
    {
        public bool Upwards { get; set; }
        public bool Downwards { get; set; }
        public bool Rightwards { get; set; }
        public bool Leftwards { get; set; }


        public byte[] Encode()
        {
            return new byte[] { 
                Convert.ToByte(Upwards),
                Convert.ToByte(Downwards),
                Convert.ToByte(Rightwards),
                Convert.ToByte(Leftwards),
            };
        }

        public static MovementStruct Decode(byte[] bs)
        {
            MovementStruct rt = new MovementStruct();
            rt.Upwards = bs[0] > 0;
            rt.Downwards = bs[1] > 0;
            rt.Rightwards = bs[2] > 0;
            rt.Leftwards = bs[3] > 0;

            return rt;
        }
    }
}
