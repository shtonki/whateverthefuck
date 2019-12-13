using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    class Panel : GUIComponent
    {
        protected Zoomer Zoomer { get; set; }

        protected MenuBar MBar { get; set; }

        public Panel() : this(new GLCoordinate(0, 0), new GLCoordinate(0, 0))
        {

        }

        public Panel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            Visible = false;
            Zoomer = new Zoomer();
        }

        public void AddMenuBar()
        {
            MBar = new MenuBar(this.Size.X);
            base.Add(MBar);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
#if false
            drawAdapter.Translate(Location.X, Location.Y);
            
            MBar?.DrawMe(drawAdapter);

            drawAdapter.Translate(-Location.X, -Location.Y);
#endif
        }

        // just fucking shoot me already
        internal class MenuBar : GUIComponent
        {
            private static float BarHeight = 0.03f;
            
            private Button CloseButton;
            private GLCoordinate ButtonSize = new GLCoordinate(BarHeight, BarHeight);

            internal MenuBar(float width) : base(new GLCoordinate(0, 0), new GLCoordinate(width, BarHeight))
            {
                CloseButton = new Button(new GLCoordinate(width-ButtonSize.X, 0), ButtonSize);
                CloseButton.BackColor = Color.Red;
#if false
                CloseButton.OnLeftMouseDown += coordinate =>
                {
                    CloseButton.BackColor = Color.Black;
                };
#endif
                Add(CloseButton);

                this.BackColor = Color.Blue;
            }

            public override void DrawMe(DrawAdapter drawAdapter)
            {
                base.DrawMe(drawAdapter);
                CloseButton.DrawMe(drawAdapter);
            }
        }
    }
    class DraggablePanel : Panel
    {
        private Panel DraggedPanel;

        //private GLCoordinate InternalOffset = new GLCoordinate(0, 0);
        private bool ListenToMouseMove = false;

        private bool Dragging;

        public DraggablePanel() : base()
        {
            DraggedPanel = new Panel();
            base.Add(DraggedPanel);

            BackColor = Color.Black;

            OnMouseButtonPress += (c, i) =>
            {
                if (i.MouseButton == OpenTK.Input.MouseButton.Left)
                {
                    Dragging = i.Direction == control.InputUnion.Directions.Down;
                }
            };

            OnMouseMove += (c, i) =>
            {
                if (Dragging)
                {
                    var dx = i.Location.X - i.PreviousLocation.X;
                    var dy = i.Location.Y - i.PreviousLocation.Y;
                    DraggedPanel.Location = new GLCoordinate(DraggedPanel.Location.X - dx, DraggedPanel.Location.Y - dy);
                }
            };
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.ActivateScissor(Location as GLCoordinate, Size);
            //drawAdapter.Scale(Zoomer.CurrentZoom, Zoomer.CurrentZoom);
            base.DrawMe(drawAdapter);
            //drawAdapter.Scale(1/Zoomer.CurrentZoom, 1/Zoomer.CurrentZoom);
            drawAdapter.DeactivateScissor();
        }

        public override void Add(GUIComponent toAdd)
        {
            DraggedPanel.Add(toAdd);
        }
    }
}
