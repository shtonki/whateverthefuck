using System;
using System.Drawing;

namespace whateverthefuck.src.model.entities
{
    public class NPC : Character
    {
        public NPC(EntityIdentifier identifier) : base(identifier, EntityType.NPC)
        {
            DrawColor = Color.Red;
            Movable = true;
        }


        public override GameCoordinate CalculateMovement()
        {
            return new GameCoordinate(0, 0);
        }
    }
}
