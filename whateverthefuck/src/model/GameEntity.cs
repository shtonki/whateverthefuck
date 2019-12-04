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

        public GameCoordinate Location { get; } = new GameCoordinate(0, 0);
        public GameCoordinate Size { get; } = new GameCoordinate(0.1f, 0.1f);
        
        // retains the last movement made
        public GameCoordinate MovementCache { get; private set; }
        public bool Movable { get; protected set; }

        public float Left => Location.X;
        public float Right => Location.X + Size.X;
        public float Bottom => Location.Y;
        public float Top => Location.Y + Size.Y;
        public GameCoordinate Center => new GameCoordinate(Location.X + Size.X / 2, Location.Y + Size.Y / 2);



        protected Color DrawColor = Color.Black;

        public GameEntity()
        {
            Location = new GameCoordinate(0, 0);
        }

        public override void Draw(DrawAdapter drawAdapter)
        {
            float x1 = Location.X;
            float y1 = Location.Y;
            float x2 = x1 + Size.X;
            float y2 = y1 + Size.Y;

            drawAdapter.fillRectangle(x1, y1, x2, y2, DrawColor);
        }

        public virtual void Step()
        {
            MovementCache = CalculateMovement();
            Location.X += MovementCache.X;
            Location.Y += MovementCache.Y;
        }


        public virtual GameCoordinate CalculateMovement()
        {
            return new GameCoordinate(0, 0);
        }
    }
}
