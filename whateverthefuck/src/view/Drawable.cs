namespace whateverthefuck.src.view
{
    using System;
    using System.Drawing;
    using whateverthefuck.src.model;
    using whateverthefuck.src.view.guicomponents;

    public abstract class Drawable
    {
        protected Drawable()
        {
        }

        public bool Visible { get; set; } = true;

        public virtual GLCoordinate Location { get; set; }

        public Sprite Sprite { get; set; }

        protected float Rotation { get; set; } = 0;

        public abstract void DrawMe(DrawAdapter drawAdapter);

        public void Draw(DrawAdapter drawAdapter)
        {
            if (this.Visible)
            {
                drawAdapter.PushMatrix();

                drawAdapter.Translate(this.Location.X, this.Location.Y);

                this.DrawMe(drawAdapter);

                drawAdapter.PopMatrix();
            }
        }
    }

    public class GUILine : GUIComponent
    {
        private float x1;
        private float y1;
        private float x2;
        private float y2;

        public GUILine(GUIComponent o, GUIComponent e)
            : base(new GLCoordinate(0, 0), new GLCoordinate(0, 0))
        {
            this.x1 = o.Location.X + (o.Size.X / 2);
            this.y1 = o.Location.Y - (o.Size.Y / 2);
            this.x2 = e.Location.X + (e.Size.X / 2);
            this.y2 = e.Location.Y - (e.Size.Y / 2);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillLine(this.x1, this.y1, this.x2, this.y2, Color.White);
        }
    }

    public class Line : Drawable
    {
        private float x1;
        private float y1;
        private float x2;
        private float y2;

        public Line(GameEntity o, GameEntity e)
        {
            this.x1 = o.Center.X;
            this.y1 = o.Center.Y;
            this.x2 = e.Center.X;
            this.y2 = e.Center.Y;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillLine(this.x1, this.y1, this.x2, this.y2, Color.White);
        }
    }

    public class Rectangle : Drawable
    {
        private float x1;
        private float y1;
        private float x2;
        private float y2;

        private Color drawColor;

        public Rectangle(GameEntity o, Color drawColor)
        {
            this.x1 = o.Left;
            this.y1 = o.Bottom;
            this.x2 = o.Right;
            this.y2 = o.Top;
            this.drawColor = drawColor;
        }

        public Rectangle(float x1, float y1, float width, float height, Color drawColor)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x1 + width;
            this.y2 = y1 + height;
            this.drawColor = drawColor;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.TraceRectangle(this.x1, this.y1, this.x2, this.y2, this.drawColor, 4);
        }
    }
}
