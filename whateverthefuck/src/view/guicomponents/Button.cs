using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    class Button : GUIComponent
    {
        private GLCoordinate Size;
        private Button(Coordinate location) : base(location)
        {
        }
        
        public Button(GLCoordinate location, GLCoordinate size) : this(location)
        {
            Size = size;
            Color = Color.DarkGoldenrod;
            OnLeftMouseDown += () =>
            {
                Logging.Log(this.GetType() + " was Left Pressed.");
            };
            OnLeftMouseUp += () =>
            {
                Logging.Log(this.GetType() + " was Left Released.");
            };
            OnRightMouseDown += () =>
            {
                Logging.Log(this.GetType() + " was Right Pressed.");
            };
            OnRightMouseUp += () =>
            {
                Logging.Log(this.GetType() + " was Right Released.");
            };
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, Color);
        }

        public override bool Contains(GLCoordinate clicked)
        {
            return (clicked.X >= Location.X && clicked.X <= Location.X + Size.X &&
                    -clicked.Y >= Location.Y && -clicked.Y <= Location.Y + Size.Y);
        }
    }
}
