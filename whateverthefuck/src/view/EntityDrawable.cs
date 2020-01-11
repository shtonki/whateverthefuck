using System;
using System.Drawing;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.view
{
    public class EntityDrawable : Drawable
    {
        private GLCoordinate size;
        private Sprite sprite;

        public EntityDrawable(GameEntity entity)
        {
            this.size = new GLCoordinate(entity.Size.X, entity.Size.Y);
            this.Location = new GLCoordinate(entity.Center.X, entity.Center.Y);
            this.Identifier = entity.Identifier;
            this.sprite = entity.Sprite;
        }

        public EntityIdentifier Identifier { get; private set; }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.DrawSprite(0, 0, size.X, size.Y, sprite);
            //drawAdapter.FillRectangle(0, 0, size.X, size.Y, Color.Black);
        }

        // @dirty feels from having this being duped from GUIComponent but i like when things work now instead of in the future
        public bool Contains(GameCoordinate clicked)
        {
            return clicked.X >= this.Location.X && clicked.X <= this.Location.X + this.size.X &&
                    clicked.Y >= this.Location.Y && clicked.Y <= this.Location.Y + this.size.Y;
        }
    }
}
