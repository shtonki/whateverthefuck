using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    abstract class Panel : GUIComponent
    {
        protected List<GUIComponent> Components = new List<GUIComponent>();
        public Panel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {

        }

        public void Add(GUIComponent toAdd)
        {
            Components.Add(toAdd);
        }

        public void Add(params GUIComponent[] toAdd)
        {
            foreach (var add in toAdd) Components.Add(add);
        }
    }
    class StaticPanel : Panel
    {
        public StaticPanel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {

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

    class DraggablePanel : Panel
    {
        private GLCoordinate clickedDownLocation = new GLCoordinate(0, 0);
        private GLCoordinate internalOffset = new GLCoordinate(0, 0);

        public DraggablePanel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            OnLeftMouseDown += coordinate => { clickedDownLocation = coordinate; };
            OnLeftMouseUp += releaseLocation =>
            {
                var diff = new GLCoordinate(clickedDownLocation.X - releaseLocation.X, clickedDownLocation.Y - releaseLocation.Y);
                Logging.Log("Diff: " + diff);
                internalOffset = new GLCoordinate(diff.X + internalOffset.X, diff.Y + internalOffset.Y);
            };
        }


        public override void DrawMe(DrawAdapter drawAdapter)
        {
            if (!Visible) return;

            base.DrawMe(drawAdapter);

            drawAdapter.Translate(Location.X - internalOffset.X, Location.Y - internalOffset.Y);

            foreach (var c in Components)
            {
                c.DrawMe(drawAdapter);
            }

            drawAdapter.Translate(-Location.X + internalOffset.X, -Location.Y + internalOffset.Y);
        }
    }
}
