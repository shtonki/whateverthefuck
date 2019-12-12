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
using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    public abstract class GUIComponent : Drawable
    {
        protected GLCoordinate Size;
        protected internal Color BackColor;
        private Border Border;
        private List<GUIComponent> Children = new List<GUIComponent>();

        public event Action<GUIComponent, InputUnion> OnMouseButtonPress;
        public event Action<GUIComponent> OnButtonPress;

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

        protected GUIComponent(GLCoordinate location, GLCoordinate size) : base(location)
        {
            BackColor = Color.Aquamarine;
            this.Size = size;
            
        }

        public void AddBorder()
        {
            Border = new Border(Color.Black);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            //if(Border != null) drawAdapter.TraceRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, Border.BorderColor, Border.Width);
            drawAdapter.FillRectangle(0, 0, Size.X, Size.Y, BackColor);


            foreach (var kiddo in Children)
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
            var glClicked = input.Location;
            var offset = new GLCoordinate(glClicked.X - Location.X, glClicked.Y - Location.Y);

            var kiddos = InteractedChildren(offset);
            
            if (kiddos.Count() > 0)
            {
                kiddos.ForEach(child => child.HandleInput(input));
            }

            if (input.IsMouseInput)
            {
                OnMouseButtonPress?.Invoke(this, input);
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
            Children.Add(toAdd);
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
                //new GLCoordinate(interactLocation.X - this.Location.X, interactLocation.Y - this.Location.Y);
            var v = Children.Where(c => c.Contains(translatedLoc)).ToList();
            return v;
        }

        public virtual bool Contains(GLCoordinate clicked)
        {
            return (clicked.X >= Location.X && clicked.X <= Location.X + Size.X &&
                    clicked.Y >= Location.Y && clicked.Y <= Location.Y + Size.Y);
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
