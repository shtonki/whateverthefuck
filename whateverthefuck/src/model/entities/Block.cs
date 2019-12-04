using System.Drawing;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    class Block : GameEntity
    {
        public Block(EntityIdentifier id, Color color) : base(ControlInfo.NoControl, id)
        {
            DrawColor = color;
        }
    }
}
