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
        public Button(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            BackColor = Color.DarkGoldenrod;
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
    }
}
