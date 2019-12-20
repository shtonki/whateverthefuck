﻿namespace whateverthefuck.src.view.guicomponents
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.control;

    public abstract class GUIComponent : Drawable
    {
        private Border border;

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

        public event Action<GUIComponent, InputUnion> OnMouseButtonDown;

        public event Action<GUIComponent, InputUnion> OnMouseButtonUp;

        public event Action<GUIComponent, InputUnion> OnKeyboardButtonDown;

        public event Action<GUIComponent, InputUnion> OnKeyboardButtonUp;

        public event Action<GUIComponent, InputUnion> OnMouseMove;

        public GLCoordinate Size { get; set; }

        public Color BackColor { get; set; }

        public List<GUIComponent> Children { get; } = new List<GUIComponent>();

        public LayoutManager LayoutManager { get; set; }

        public void AddBorder()
        {
            this.border = new Border(Color.Black);
        }

        public virtual void AddChild(GUIComponent child)
        {
            if (this.LayoutManager != null)
            {
                this.LayoutManager.Layout(this, child);
            }

            this.Children.Add(child);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            if (this.Sprite != null)
            {
                drawAdapter.DrawSprite(0, 0, this.Size.X, this.Size.Y, this.Sprite);
            }
            else
            {
                drawAdapter.FillRectangle(0, 0, this.Size.X, this.Size.Y, this.BackColor);
            }

            foreach (var kiddo in this.Children)
            {
                drawAdapter.PushMatrix();
                drawAdapter.Translate(kiddo.Location.X, kiddo.Location.Y);

                kiddo.DrawMe(drawAdapter);

                drawAdapter.PopMatrix();
            }
        }

        public void HandleInput(InputUnion input)
        {
            this.HandleInput(input, new GLCoordinate(this.Location.X, this.Location.Y));
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
