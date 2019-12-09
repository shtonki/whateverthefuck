using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.view.guicomponents
{
    class Frame : GUIComponent
    {
        List<GUIComponent> Components = new List<GUIComponent>();
        
        public Frame(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
        }

        public void Add(GUIComponent toAdd)
        {
            Components.Add(toAdd);
        }

        public void Add(params GUIComponent [] toAdd)
        {
            foreach(var add in toAdd) Components.Add(add);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            if (!Visible) return;

            base.DrawMe(drawAdapter);

            drawAdapter.Translate(Location.X, Location.Y);

            foreach (var c in Components)
            {
                c.DrawMe(drawAdapter);
            }

            drawAdapter.Translate(-Location.X, -Location.Y);
        }
    }
}
