﻿using System;
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

        public DraggablePanel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            DraggedPanel = new Panel(new GLCoordinate(0, 0), size);
            DraggedPanel.BackColor = Color.Pink;
            BackColor = Color.White;
            base.Add(DraggedPanel);
#if false

            OnLeftMouseDown += coordinate =>
            {
                ListenToMouseMove = true;
            };

            OnMouseMove += coordinate =>
            {
                if (!ListenToMouseMove) return;
                var diff = coordinate;
                diff.X += 1;
                diff.Y -= 1;
                DraggedPanel.Location = new GLCoordinate(DraggedPanel.Location.X + diff.X, DraggedPanel.Location.Y + diff.Y);
                //InternalOffset = new GLCoordinate(-diff.X + InternalOffset.X, diff.Y + InternalOffset.Y);
            };

            OnLeftMouseUp += releaseLocation =>
            {
                ListenToMouseMove = false;
            };

            OnScrollIn += coordinate =>
            {
                Zoomer.ZoomIn();
            };

            OnScrollOut += coordinate =>
            {
                Zoomer.ZoomOut();
            };
#endif
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            var locScreenCoords = GUI.TranslateGLToScreenCoordinates(new GLCoordinate(0, 0));
            var sizeScreenCoords = GUI.TranslateGLToScreenCoordinates(new GLCoordinate(Size.X - 1, -Size.Y + 1));


            //drawAdapter.ActivateScissor(locScreenCoords.X, locScreenCoords.Y, sizeScreenCoords.X, -sizeScreenCoords.Y);
            //drawAdapter.ActivateScissor(locScreenCoords.X, locScreenCoords.Y, sizeScreenCoords.X, sizeScreenCoords.Y);
            drawAdapter.ActivateScissor(locScreenCoords.X, locScreenCoords.Y, sizeScreenCoords.X, sizeScreenCoords.Y);

            //drawAdapter.Translate(-InternalOffset.X, -InternalOffset.Y);

            //drawAdapter.Scale(Zoomer.CurrentZoom, Zoomer.CurrentZoom);
            
            base.DrawMe(drawAdapter);

            //DraggedPanel.DrawMe(drawAdapter);

            //drawAdapter.Scale(1/Zoomer.CurrentZoom, 1/Zoomer.CurrentZoom);

            //drawAdapter.Translate(InternalOffset.X, InternalOffset.Y);

            drawAdapter.DeactivateScissor();
        }

        public override void Add(GUIComponent toAdd)
        {
            DraggedPanel.Add(toAdd);
        }
    }
}
