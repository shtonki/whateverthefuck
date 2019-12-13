namespace whateverthefuck.src.view.guicomponents
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.control;

    public abstract class GUIComponent : Drawable
    {
        public GLCoordinate Size { get; set; }

        protected internal Color BackColor;
        private Border Border;
        private List<GUIComponent> Children = new List<GUIComponent>();

        public event Action<GUIComponent, InputUnion> OnMouseButtonPress;

        public event Action<GUIComponent, InputUnion> OnKeyboardButtonPress;

        public event Action<GUIComponent, InputUnion> OnMouseMove;

#if false
        protected void LeftMouseDown(GLCoordinate glClicked)
        {
            InteractedChildren(glClicked).ForEach(child => child.LeftMouseDown(glClicked));
            OnLeftMouseDown?.Invoke(glClicked);
        }

        protected void LeftMouseUp(GLCoordinate glClicked)
        {
            InteractedChildren(glClicked).ForEach(child => child.LeftMouseUp(glClicked));
            OnLeftMouseUp?.Invoke(glClicked);
        }

        protected void RightMouseDown(GLCoordinate glClicked)
        {
            InteractedChildren(glClicked).ForEach(child => child.RightMouseDown(glClicked));
            OnRightMouseDown?.Invoke(glClicked);
        }

        protected void RightMouseUp(GLCoordinate glClicked)
        {
            InteractedChildren(glClicked).ForEach(child => child.RightMouseUp(glClicked));
            OnRightMouseUp?.Invoke(glClicked);
        }

        protected void MouseMove(GLCoordinate delta)
        {
            OnMouseMove?.Invoke(delta);
        }

        protected void ScrollIn(GLCoordinate glMouseLocation)
        {
            OnScrollIn?.Invoke(glMouseLocation);
        }

        protected void ScrollOut(GLCoordinate glMouseLocation)
        {
            OnScrollOut?.Invoke(glMouseLocation);
        }
#endif
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

        public void AddBorder()
        {
            this.Border = new Border(Color.Black);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            // if(Border != null) drawAdapter.TraceRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, Border.BorderColor, Border.Width);
            drawAdapter.FillRectangle(0, 0, this.Size.X, this.Size.Y, this.BackColor);

            foreach (var kiddo in this.Children)
            {
                drawAdapter.PushMatrix();
                drawAdapter.Translate(kiddo.Location.X, kiddo.Location.Y);

                kiddo.DrawMe(drawAdapter);

                drawAdapter.PopMatrix();
            }

            //drawAdapter.Translate(-this.Location.X, -this.Location.Y);
        }

        public void HandleInput(InputUnion input)
        {
            List<GUIComponent> kiddos;

            if (input.IsMouseInput)
            {
                var glClicked = input.Location;
                var offset = new GLCoordinate(glClicked.X - this.Location.X, glClicked.Y - this.Location.Y);

                kiddos = this.InteractedChildren(offset);
            }
            else
            {
                kiddos = this.Children;
            }

            if (kiddos.Count() > 0)
            {
                kiddos.ForEach(child => child.HandleInput(input));
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

#if false
        public void HandleClick(InputUnion mouseInput, GLCoordinate glClicked)
        {
            if (mouseInput.Direction == InputUnion.Directions.Up && mouseInput.MouseButton == MouseButton.Left) LeftMouseUp(glClicked);
            else if (mouseInput.Direction == InputUnion.Directions.Down && mouseInput.MouseButton == MouseButton.Left) LeftMouseDown(glClicked);

            if (mouseInput.Direction == InputUnion.Directions.Up && mouseInput.MouseButton == MouseButton.Right) RightMouseUp(glClicked);
            else if (mouseInput.Direction == InputUnion.Directions.Down && mouseInput.MouseButton == MouseButton.Right) RightMouseDown(glClicked);
        }

        public void HandleScroll(InputUnion mouseInput, GLCoordinate glMouseLocation)
        {
            if (mouseInput.Direction == InputUnion.Directions.Down)
            {
                ScrollIn(glMouseLocation);
            }
            else
            {
                ScrollOut(glMouseLocation);
            }
        }

        public void HandleMouseMove(GLCoordinate delta)
        {
            MouseMove(delta);
        }
#endif
        public virtual void Add(GUIComponent toAdd)
        {
            this.Children.Add(toAdd);
        }

#if false
        public virtual void Add(params GUIComponent[] toAdd)
        {
            foreach (var add in toAdd)
            {
                Add(add);
            }
        }
#endif
        public List<GUIComponent> InteractedChildren(GLCoordinate interactLocation)
        {
            var translatedLoc = interactLocation;
            var v = this.Children.Where(c => c.Contains(translatedLoc)).ToList();
            return v;
        }

        public virtual bool Contains(GLCoordinate clicked)
        {
            return clicked.X >= this.Location.X && clicked.X <= this.Location.X + this.Size.X &&
                    clicked.Y >= this.Location.Y && clicked.Y <= this.Location.Y + this.Size.Y;
        }
    }

    internal class Border
    {
        internal Color BorderColor { get; set; } = Color.Black;

        internal float Width { get; set; }

        internal Border(Color borderColor, float width = 4f)
        {
            this.BorderColor = borderColor;
            this.Width = width;
        }
    }
}
