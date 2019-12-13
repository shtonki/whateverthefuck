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
    internal class Button : GUIComponent
    {
        public Button() : base()
        {
        }

        public Button(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            BackColor = Color.DarkGoldenrod;

            OnMouseButtonPress += (c, i) =>
            {
                if (i.Direction == control.InputUnion.Directions.Down)
                {
                    Logging.Log("down");
                }
                else if (i.Direction == control.InputUnion.Directions.Up)
                {
                    Logging.Log("up");
                }
            };

#if false
            OnLeftMouseDown += (a) =>
            {
                Logging.Log(this.GetType() + " was Left Pressed.");
            };
            OnLeftMouseUp += (a) =>
            {
                Logging.Log(this.GetType() + " was Left Released.");
            };
            OnRightMouseDown += (a) =>
            {
                Logging.Log(this.GetType() + " was Right Pressed.");
            };
            OnRightMouseUp += (a) =>
            {
                Logging.Log(this.GetType() + " was Right Released.");
            };
#endif
        }
    }
}
