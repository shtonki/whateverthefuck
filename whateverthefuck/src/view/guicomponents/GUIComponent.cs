using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view.guicomponents
{
    public abstract class GUIComponent : Drawable
    {
        private static GLCoordinate DefaultSize = new GLCoordinate(0.1f, 0.1f);
        protected GLCoordinate Size;
        protected Color BackColor;
        private Border Border;
        public Action<GLCoordinate> OnLeftMouseDown   = (clickedLocation) => { };
        public Action<GLCoordinate> OnLeftMouseUp     = (clickedLocation) => { };
        public Action<GLCoordinate> OnRightMouseDown  = (clickedLocation) => { };
        public Action<GLCoordinate> OnRightMouseUp    = (clickedLocation) => { };

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
