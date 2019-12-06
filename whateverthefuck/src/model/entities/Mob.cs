using System;
using System.Drawing;

namespace whateverthefuck.src.model.entities
{
    class Mob : GameEntity
    {
        public Mob(EntityIdentifier identifier) : base(identifier)
        {
            DrawColor = Color.Red;
            Movable = true;
        }

        int ctr = 0;

        public override GameCoordinate CalculateMovement()
        {
            if (ctr++ % 100 < 50)
            {
                return new GameCoordinate(0, 0.005f);
            }
            else
            {
                return new GameCoordinate(0, -0.005f);
            }
        }
    }
}
