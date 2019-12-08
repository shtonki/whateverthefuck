using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillRectangle(Location.X, Location.Y, Location.X + Size.X, Location.Y + Size.Y, Color);
        }
    }
}
