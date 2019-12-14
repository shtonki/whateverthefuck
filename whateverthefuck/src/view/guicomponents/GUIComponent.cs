namespace whateverthefuck.src.view.guicomponents
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.control;

    public abstract class GUIComponent : Drawable
    {
        private Border border;
        protected List<GUIComponent> children = new List<GUIComponent>();

        protected GUIComponent()
            : this(new GLCoordinate(0, 0), new GLCoordinate(0, 0))
        {
        }

        protected GUIComponent(GLCoordinate location, GLCoordinate size)
            : base(location)
        {
            this.BackColor = Color.Aquamarine;
            this.Size = size;
        }

        public event Action<GUIComponent, InputUnion> OnMouseButtonPress;

        public event Action<GUIComponent, InputUnion> OnKeyboardButtonPress;

        public event Action<GUIComponent, InputUnion> OnMouseMove;

        public GLCoordinate Size { get; set; }

        public Color BackColor { get; set; }

        public void AddBorder()
        {
            this.border = new Border(Color.Black);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            // if(Border != null) drawAdapter.TraceRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, Border.BorderColor, Border.Width);
            drawAdapter.FillRectangle(0, 0, this.Size.X, this.Size.Y, this.BackColor);

            foreach (var kiddo in this.children)
            {
                drawAdapter.PushMatrix();
                drawAdapter.Translate(kiddo.Location.X, kiddo.Location.Y);

                kiddo.DrawMe(drawAdapter);

                drawAdapter.PopMatrix();
            }
        }

        public void HandleInput(InputUnion input)
        {
            HandleInput(input, new GLCoordinate(this.Location.X, this.Location.Y));
        }

        protected void HandleInput(InputUnion input, GLCoordinate initialOffset)
        {
            if (input.IsMouseInput)
            {
                var glClicked = input.Location;
                var internalClickLocation = new GLCoordinate(glClicked.X - initialOffset.X, glClicked.Y - initialOffset.Y);

                var clickedChild = this.children.FirstOrDefault(c => c.Contains(internalClickLocation));

                if (clickedChild != null)
                {
                    var newOffset = new GLCoordinate(initialOffset.X + clickedChild.Location.X, initialOffset.Y + clickedChild.Location.Y);

                    clickedChild.HandleInput(input, newOffset);
                    return;
                }
            }

            if (input.IsMouseInput)
            {
                this.OnMouseButtonPress?.Invoke(this, input);
            }
            else if (input.IsKeyboardInput)
            {
                this.OnKeyboardButtonPress?.Invoke(this, input);
            }
            else if (input.IsMouseMove)
            {
                this.OnMouseMove?.Invoke(this, input);
            }
        }

        public virtual void Add(GUIComponent toAdd)
        {
            this.children.Add(toAdd);
        }

        public virtual bool Contains(GLCoordinate clicked)
        {
            return clicked.X >= this.Location.X && clicked.X <= this.Location.X + this.Size.X &&
                    clicked.Y >= this.Location.Y && clicked.Y <= this.Location.Y + this.Size.Y;
        }
    }

    internal class Border
    {
        internal Border(Color borderColor, float width = 4f)
        {
            this.BorderColor = borderColor;
            this.Width = width;
        }

        internal Color BorderColor { get; set; } = Color.Black;

        internal float Width { get; set; }
    }
}
