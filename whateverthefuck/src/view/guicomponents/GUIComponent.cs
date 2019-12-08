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
        protected Color Color;
        public Action OnLeftMouseDown = () => { };
        public Action OnLeftMouseUp = () => { };
        public Action OnRightMouseDown= () => { };
        public Action OnRightMouseUp = () => { };
        protected GUIComponent(Coordinate location) : base(location)
        {
        }
        public abstract bool Contains(GLCoordinate clicked);
    }
}
