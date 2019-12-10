using System;
using System.Drawing;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    public class NPC : Character
    {
        public NPC(EntityIdentifier identifier) : base(identifier, EntityType.NPC)
        {
            DrawColor = Color.Red;
            Movable = true;
            MoveSpeed = 0.001f;
        }

    }
}
