namespace whateverthefuck.src.view.guicomponents
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.control;

    public abstract class GUIComponent : Drawable
    {
        protected GUIComponent()
            : this(new GLCoordinate(0, 0), new GLCoordinate(0, 0))
        {
        }

        protected GUIComponent(GLCoordinate location, GLCoordinate size)
        {
            this.BackColor = Color.Aquamarine;
            this.Size = size;
            this.Location = location;
        }

        public event Action<GUIComponent, InputUnion> OnMouseButtonDown;

        public event Action<GUIComponent, InputUnion> OnMouseButtonUp;

        public event Action<GUIComponent, InputUnion> OnKeyboardButtonDown;

        public event Action<GUIComponent, InputUnion> OnKeyboardButtonUp;

        public event Action<GUIComponent, InputUnion> OnMouseMove;

        public GLCoordinate Size { get; set; }

        public Color BackColor { get; set; }

        public List<GUIComponent> Children { get; } = new List<GUIComponent>();

        public LayoutManager LayoutManager { get; set; }

        public Border Border { get; set; }

        public bool DestroyMe { get; protected set; }

        public virtual void Step()
        {

        }

        public void AddBorder(Color color)
        {
            this.Border = new Border(color);
        }

        public void AddChild(GUIComponent child)
        {
            if (this.LayoutManager != null)
            {
                this.LayoutManager.Layout(this, child);
            }

            this.Children.Add(child);
        }

        public void RemoveChild(GUIComponent child)
        {
            this.Children.Remove(child);
        }

        public void ClearChildren()
        {
            this.Children.Clear();
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            if (this.Sprite != null)
            {
                drawAdapter.DrawSprite(0, 0, this.Size.X, this.Size.Y, this.Sprite);
            }
            else if (this.BackColor != Color.Transparent)
            {
                drawAdapter.FillRectangle(0, 0, this.Size.X, this.Size.Y, this.BackColor);
            }

            if (this.Border != null)
            {
                drawAdapter.TraceRectangle(0, 0, this.Size.X, this.Size.Y, this.Border.BorderColor, this.Border.Width);
            }

            foreach (var kiddo in this.Children)
            {
                kiddo.Draw(drawAdapter);
            }
        }

        public void HandleInput(InputUnion input)
        {
            this.HandleInput(input, new GLCoordinate(this.Location.X, this.Location.Y));
        }

        public void Destroy()
        {
            DestroyMe = true;
            Visible = false;
        }

        public virtual bool Contains(GLCoordinate clicked)
        {
            return clicked.X >= this.Location.X && clicked.X <= this.Location.X + this.Size.X &&
                    clicked.Y >= this.Location.Y && clicked.Y <= this.Location.Y + this.Size.Y;
        }

        protected void HandleInput(InputUnion input, GLCoordinate initialOffset)
        {
            if (input.IsMouseInput)
            {
                var glClicked = input.Location;
                var internalClickLocation = new GLCoordinate(glClicked.X - initialOffset.X, glClicked.Y - initialOffset.Y);

                var clickedChild = this.Children.FirstOrDefault(c => c.Contains(internalClickLocation));

                if (clickedChild != null)
                {
                    var newOffset = new GLCoordinate(initialOffset.X + clickedChild.Location.X, initialOffset.Y + clickedChild.Location.Y);

                    clickedChild.HandleInput(input, newOffset);
                    return;
                }
            }

            if (input.IsMouseInput)
            {
                if (input.Direction == InputUnion.Directions.Down)
                {
                    this.OnMouseButtonDown?.Invoke(this, input);
                }
                else if (input.Direction == InputUnion.Directions.Up)
                {
                    this.OnMouseButtonUp?.Invoke(this, input);
                }
            }
            else if (input.IsKeyboardInput)
            {
                if (input.Direction == InputUnion.Directions.Down)
                {
                    this.OnKeyboardButtonDown?.Invoke(this, input);
                }
                else if (input.Direction == InputUnion.Directions.Up)
                {
                    this.OnKeyboardButtonUp?.Invoke(this, input);
                }
            }
            else if (input.IsMouseMove)
            {
                this.OnMouseMove?.Invoke(this, input);
            }
        }

    }

    public class Border
    {
        internal Border(Color borderColor, float width = 4f)
        {
            this.BorderColor = borderColor;
            this.Width = width;
        }

        internal Color BorderColor { get; set; } = Color.Black;

        internal float Width { get; set; }
    }

    public interface ToolTipper
    {
        string GetToolTip();
    }
}
