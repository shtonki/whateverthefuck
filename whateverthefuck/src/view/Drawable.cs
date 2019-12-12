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


    public class Rectangle : Drawable
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        public Color DrawColor;

        public Rectangle(GameEntity o, Color drawColor) : base(new GameCoordinate(0, 0))
        {
            X1 = o.Left;
            Y1 = o.Bottom;
            X2 = o.Right;
            Y2 = o.Top;
            DrawColor = drawColor;
        }

        public Rectangle(float x1, float y1, float x2, float y2, Color drawColor) : base(new GameCoordinate(0, 0))
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            DrawColor = drawColor;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.TraceRectangle(X1, Y1, X2, Y2, DrawColor, 4);
        }
    }

    public abstract class Drawable
    {
        public int Height { get; protected set; } = 1;
        
        protected float Rotation { get; set; } = 0;
        public Coordinate Location { get; set; }

        public bool Visible { get; set; } = true;

        public abstract void DrawMe(DrawAdapter drawAdapter);
        public void Draw(DrawAdapter drawAdapter)
        {

            if (Visible)
            {
                drawAdapter.PushMatrix();

                var l = Location.ToGLCoordinate();
                drawAdapter.Translate(l.X, l.Y);

                DrawMe(drawAdapter);

                drawAdapter.PopMatrix();
            }

        }


        protected Drawable(Coordinate location)
        {
            Location = location;
        }
    }

}
