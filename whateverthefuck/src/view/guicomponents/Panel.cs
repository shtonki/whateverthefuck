﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    abstract class Panel : GUIComponent
    {
        public Zoomer Zoomer { get; set; }
        protected List<GUIComponent> Components = new List<GUIComponent>();
        public Panel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            Visible = false;
            Zoomer = new Zoomer();
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
        private GLCoordinate InternalOffset = new GLCoordinate(0, 0);
        private bool ListenToMouseMove = false;

        public DraggablePanel(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            OnLeftMouseDown += coordinate =>
            {
                ListenToMouseMove = true;
            };

            OnMouseMove += coordinate =>
            {
                if (!ListenToMouseMove) return;
                var diff = coordinate;
                diff.X += 1;
                diff.Y += 1;
                InternalOffset = new GLCoordinate(-diff.X + InternalOffset.X, diff.Y + InternalOffset.Y);
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
        }


        public override void DrawMe(DrawAdapter drawAdapter)
        {
            if (!Visible) return;

            base.DrawMe(drawAdapter);

            var locScreenCoords = GUI.TranslateGLToScreenCoordinates(Location as GLCoordinate);
            var sizeScreenCoords = GUI.TranslateGLToScreenCoordinates(new GLCoordinate(Size.X - 1, Size.Y - 1));

            drawAdapter.ActivateScissor(locScreenCoords.X, locScreenCoords.Y, sizeScreenCoords.X, sizeScreenCoords.Y);

            drawAdapter.Translate(Location.X - InternalOffset.X, Location.Y - InternalOffset.Y);

            drawAdapter.Scale(Zoomer.CurrentZoom, Zoomer.CurrentZoom);

            foreach (var c in Components)
            {
                c.DrawMe(drawAdapter);
            }

            drawAdapter.Scale(1/Zoomer.CurrentZoom, 1/Zoomer.CurrentZoom);

            drawAdapter.Translate(-Location.X + InternalOffset.X, -Location.Y + InternalOffset.Y);

            drawAdapter.DeactivateScissor();
        }
    }
}
