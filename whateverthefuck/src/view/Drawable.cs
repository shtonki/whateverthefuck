namespace whateverthefuck.src.view
{
    using System.Drawing;
    using whateverthefuck.src.model;

    public class Line : Drawable
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        public Line(GameEntity o, GameEntity e)
            : base(new GameCoordinate(0, 0))
        {
            this.X1 = o.Center.X;
            this.Y1 = o.Center.Y;
            this.X2 = e.Center.X;
            this.Y2 = e.Center.Y;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillLine(this.X1, this.Y1, this.X2, this.Y2, Color.White);
        }
    }

    public class Rectangle : Drawable
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        public Color DrawColor;

        public Rectangle(GameEntity o, Color drawColor)
            : base(new GameCoordinate(0, 0))
        {
            this.X1 = o.Left;
            this.Y1 = o.Bottom;
            this.X2 = o.Right;
            this.Y2 = o.Top;
            this.DrawColor = drawColor;
        }

        public Rectangle(float x1, float y1, float x2, float y2, Color drawColor)
            : base(new GameCoordinate(0, 0))
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
            this.DrawColor = drawColor;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.TraceRectangle(this.X1, this.Y1, this.X2, this.Y2, this.DrawColor, 4);
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
            if (this.Visible)
            {
                drawAdapter.PushMatrix();

                var l = this.Location.ToGLCoordinate();
                drawAdapter.Translate(l.X, l.Y);

                this.DrawMe(drawAdapter);

                drawAdapter.PopMatrix();
            }
        }

        protected Drawable(Coordinate location)
        {
            this.Location = location;
        }
    }
}
