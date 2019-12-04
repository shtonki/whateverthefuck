using System;
using System.Drawing;

namespace whateverthefuck.src.model.entities
{
    class Mob : GameEntity
    {
        public Mob(EntityIdentifier identifier) : base(ControlInfo.ServerControl, identifier)
        {
            DrawColor = Color.Red;
        }
    }
}
