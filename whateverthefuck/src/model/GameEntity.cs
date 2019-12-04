using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class GameEntity : Drawable
    {
        protected Color DrawColor = Color.Black;

        public GameEntity()
        {
            Location = new GameCoordinate(0, 0);
        }

        public override void Draw(DrawAdapter drawAdapter)
        {
            float x1 = Location.X;
            float y1 = Location.Y;
            float x2 = x1 + 0.1f;
            float y2 = y1 + 0.1f;

            drawAdapter.fillRectangle(x1, y1, x2, y2, DrawColor);
        }

        public virtual void Step()
        {
            var movement = CalculateMovement();
        }

        public virtual GameCoordinate CalculateMovement()
        {
            return new GameCoordinate(0, 0);
        }
    }
}
