namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;

    internal class Panel : GUIComponent
    {
        protected Zoomer Zoomer { get; set; }

        protected MenuBar MBar { get; set; }

        public Panel()
            : this(new GLCoordinate(0, 0), new GLCoordinate(0, 0))
        {
        }

        public Panel(GLCoordinate location, GLCoordinate size)
            : base(location, size)
        {
            this.Visible = false;
            this.Zoomer = new Zoomer();
        }

        public void AddMenuBar()
        {
            this.MBar = new MenuBar(this.Size.X);
            base.Add(this.MBar);
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

            internal MenuBar(float width)
                : base(new GLCoordinate(0, 0), new GLCoordinate(width, BarHeight))
            {
                this.CloseButton = new Button(new GLCoordinate(width - this.ButtonSize.X, 0), this.ButtonSize);
                this.CloseButton.BackColor = Color.Red;
#if false
                CloseButton.OnLeftMouseDown += coordinate =>
                {
                    CloseButton.BackColor = Color.Black;
                };
#endif
                this.Add(this.CloseButton);

                this.BackColor = Color.Blue;
            }

            public override void DrawMe(DrawAdapter drawAdapter)
            {
                base.DrawMe(drawAdapter);
                this.CloseButton.DrawMe(drawAdapter);
            }
        }
    }

    internal class DraggablePanel : Panel
    {
        private Panel DraggedPanel;

        private bool Dragging;

        public DraggablePanel()
            : base()
        {
            this.DraggedPanel = new Panel();
            base.Add(this.DraggedPanel);

            this.BackColor = Color.Black;

            this.OnMouseButtonPress += (c, i) =>
            {
                if (i.MouseButton == OpenTK.Input.MouseButton.Left)
                {
                    this.Dragging = i.Direction == control.InputUnion.Directions.Down;
                }
            };

            this.OnMouseMove += (c, i) =>
            {
                if (this.Dragging)
                {
                    var dx = i.Location.X - i.PreviousLocation.X;
                    var dy = i.Location.Y - i.PreviousLocation.Y;
                    this.DraggedPanel.Location = new GLCoordinate(this.DraggedPanel.Location.X - dx, this.DraggedPanel.Location.Y - dy);
                }
            };
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.ActivateScissor(this.Location as GLCoordinate, this.Size);
            //drawAdapter.Scale(Zoomer.CurrentZoom, Zoomer.CurrentZoom);
            base.DrawMe(drawAdapter);
            //drawAdapter.Scale(1/Zoomer.CurrentZoom, 1/Zoomer.CurrentZoom);
            drawAdapter.DeactivateScissor();
        }

        public override void Add(GUIComponent toAdd)
        {
            this.DraggedPanel.Add(toAdd);
        }
    }
}
