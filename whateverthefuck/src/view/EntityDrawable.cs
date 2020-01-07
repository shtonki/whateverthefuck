using System.Drawing;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view
{
    public class EntityDrawable : Drawable
    {
        private GLCoordinate size;

        public EntityDrawable(GameEntity entity)
        {
            this.size = new GLCoordinate(entity.Size.X, entity.Size.Y);
            this.Location = new GLCoordinate(entity.GameLocation.X, entity.GameLocation.Y);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillRectangle(0, 0, size.X, size.Y, Color.Black);
        }
    }
}
