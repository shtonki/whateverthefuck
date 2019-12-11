using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using whateverthefuck.src.control;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view.guicomponents
{
    public abstract class GUIComponent : Drawable
    {
        protected GLCoordinate Size;
        protected Color BackColor;
        private Border Border;

        public event Action<GLCoordinate> OnLeftMouseDown;
        public event Action<GLCoordinate> OnLeftMouseUp;
        public event Action<GLCoordinate> OnRightMouseDown;
        public event Action<GLCoordinate> OnRightMouseUp;
        public event Action<GLCoordinate> OnMouseMove;
        public event Action<GLCoordinate> OnScrollIn;
        public event Action<GLCoordinate> OnScrollOut;


        protected void LeftMouseDown(GLCoordinate glClicked)
        {
            OnLeftMouseDown?.Invoke(glClicked);
        }

        protected void LeftMouseUp(GLCoordinate glClicked)
        {
            OnLeftMouseUp?.Invoke(glClicked);
        }

        protected void RightMouseDown(GLCoordinate glClicked)
        {
            OnRightMouseDown?.Invoke(glClicked);
        }

        protected void RightMouseUp(GLCoordinate glClicked)
        {
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


        protected GUIComponent(GLCoordinate location, GLCoordinate size) : base(location)
        {
            BackColor = Color.Aquamarine;
            this.Size = size;
            Border = new Border(Color.Black);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, BackColor);
            drawAdapter.TraceRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, Border.BorderColor, Border.Width);
        }
        
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

        public virtual bool Contains(GLCoordinate clicked)
        {
            return (clicked.X >= Location.X && clicked.X <= Location.X + Size.X &&
                    -clicked.Y >= Location.Y && -clicked.Y <= Location.Y + Size.Y);
        }
    }

    class Border
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
