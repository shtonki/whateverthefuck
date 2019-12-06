using System.Drawing;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    class Block : GameEntity
    {
        public Block(EntityIdentifier id, Color color) : base(id)
        {
            DrawColor = color;
        }
    }
}
