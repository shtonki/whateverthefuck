using System.Drawing;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public class Block : GameEntity
    {
        public Block(EntityIdentifier id, Color color) : base(id, EntityType.Block)
        {
            DrawColor = color;
            Height = 3;
        }
    }
}
