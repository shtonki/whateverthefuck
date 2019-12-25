namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.util;

    internal class Panel : GUIComponent
    {
        // @why we call panel with size 0?
        public Panel()
            : this(new GLCoordinate(0, 0), new GLCoordinate(0, 0))
        {
        }

        public Panel(GLCoordinate location, GLCoordinate size)
            : base(location, size)
        {
            this.Zoomer = new Zoomer();
        }

        protected Zoomer Zoomer { get; set; }

        protected MenuBar MBar { get; set; }

        public void AddMenuBar()
        {
            this.MBar = new MenuBar(this.Size.X);
            this.AddChild(this.MBar);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
        }

        internal class MenuBar : GUIComponent
        {
            private static float barHeight = 0.03f;

            private Button closeButton;
            private GLCoordinate buttonSize = new GLCoordinate(barHeight, barHeight);

            internal MenuBar(float width)
                : base(new GLCoordinate(0, 0), new GLCoordinate(width, barHeight))
            {
                this.closeButton = new Button(new GLCoordinate(width - this.buttonSize.X, 0), this.buttonSize);
                this.closeButton.BackColor = Color.Red;
#if false
                CloseButton.OnLeftMouseDown += coordinate =>
                {
                    CloseButton.BackColor = Color.Black;
                };
#endif
                this.AddChild(this.closeButton);

                this.BackColor = Color.Blue;
            }

            public override void DrawMe(DrawAdapter drawAdapter)
            {
                base.DrawMe(drawAdapter);
                this.closeButton.DrawMe(drawAdapter);
            }
        }
    }

    internal class DraggablePanel : Panel
    {
        private bool dragging;

        public DraggablePanel()
            : base()
        {
#if false
            this.draggedPanel = new Panel();
            this.draggedPanel.Size = new GLCoordinate(1, 1);
            this.draggedPanel.BackColor = Color.AntiqueWhite;
            base.Add(this.draggedPanel);

#endif
            this.BackColor = Color.Black;

            this.OnMouseButtonDown += (c, i) =>
            {
                if (i.MouseButton == OpenTK.Input.MouseButton.Left)
                {
                    this.dragging = true;
                }
            };

            this.OnMouseButtonUp += (c, i) =>
            {
                if (i.MouseButton == OpenTK.Input.MouseButton.Left)
                {
                    this.dragging = false;
                }
            };

            this.OnMouseMove += (c, i) =>
            {
                if (this.dragging)
                {
                    var dx = i.PreviousLocation.X - i.Location.X;
                    var dy = i.PreviousLocation.Y - i.Location.Y;
                    var dcoordinate = new GLCoordinate(dx, dy);

                    foreach (var kid in this.Children)
                    {
                        kid.Location = (GLCoordinate)kid.Location + dcoordinate;
                    }
                }
            };
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.ActivateScissor(this.Location as GLCoordinate, this.Size);
            base.DrawMe(drawAdapter);
            drawAdapter.DeactivateScissor();
        }
#if false
        public override void Add(GUIComponent toAdd)

        {
            this.draggedPanel.Add(toAdd);
        }
#endif
    }
}
