using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view
{
    public class Line : Drawable
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        public Line(GameEntity o, GameEntity e) : base(new GameCoordinate(0, 0))
        {
            X1 = o.Center.X;
            Y1 = o.Center.Y;
            X2 = e.Center.X;
            Y2 = e.Center.Y;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillLine(X1, Y1, X2, Y2, Color.White);
        }
    }

    public abstract class Drawable
    {
        protected float Rotation { get; set; } = 0;
        public Coordinate Location { get; set; }

        public bool Invisible { get; set; }

        public abstract void DrawMe(DrawAdapter drawAdapter);
        public void Draw(DrawAdapter drawAdapter)
        {
            if (!Invisible)
            {
                DrawMe(drawAdapter);
            }
        }


        protected Drawable(Coordinate location)
        {
            Location = location;
        }
    }
}
