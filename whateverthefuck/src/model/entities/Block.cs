using System.Drawing;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    class Block : GameEntity
    {
        public Block(EntityIdentifier id, Color color) : base(ControlInfo.NoControl, id)
        {
            DrawColor = color;
        }
    }
}
