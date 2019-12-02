using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    class MovementStruct
    {
        public bool Upwards { get; set; }
        public bool Downwards { get; set; }
        public bool Rightwards { get; set; }
        public bool Leftwards { get; set; }

    }

    class Character : GameEntity
    {
        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed = 5.1f;

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

        public override void Step()
        {
            base.Step();

            if (Movements.Upwards)
            {
                y += MoveSpeed;
            }

            if (Movements.Downwards)
            {
                y -= MoveSpeed;
            }

            if (Movements.Leftwards)
            {
                x -= MoveSpeed;
            }

            if (Movements.Rightwards)
            {
                x += MoveSpeed;
            }
        }
    }
}
