using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using whateverthefuck.src.control;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view
{
    class GibbWindow : GameWindow
    {

        private ManualResetEventSlim LoadResetEvent;

        public GibbWindow(ManualResetEventSlim loadResetEvent) : base(600, 600, new GraphicsMode(32, 24, 0, 32), "GibbWindow")
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            LoadResetEvent = loadResetEvent;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (LoadResetEvent != null) LoadResetEvent.Set();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.ClearColor(Color.Fuchsia);
            GL.PushMatrix();
            if (GUI.Camera != null)
            {
                GL.Scale(GUI.Camera.Zoom.CurrentZoom, GUI.Camera.Zoom.CurrentZoom, 0);
                GL.Translate(-GUI.Camera.Location.X, -GUI.Camera.Location.Y, 0);
            }

            var drawAdapter = new DrawAdapter();

            foreach (var drawable in GUI.GetAllDrawables())
            {
                drawable.Draw(drawAdapter);
            }

            this.SwapBuffers();
            GL.PopMatrix();
        }



        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                Input.Handle(new InputUnion(InputUnion.Directions.Down, MouseButton.Middle));
            }
            else
            {
                Input.Handle(new InputUnion(InputUnion.Directions.Up, MouseButton.Middle));
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var gl = new GLCoordinate(e.X * 2.0f / ClientSize.Width - 1, e.Y * 2.0f / ClientSize.Height - 1);
            GameCoordinate gc = GUI.Camera.GLToGameCoordinate(gl);
            
            Program.GameStateManager.HandleClick(e, gc);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            var gl = new GLCoordinate(e.X * 2.0f / ClientSize.Width - 1, e.Y * 2.0f / ClientSize.Height - 1);
            GameCoordinate gc = GUI.Camera.GLToGameCoordinate(gl);

            Program.GameStateManager.HandleClick(e, gc);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            Input.Handle(new InputUnion(e.IsRepeat ? InputUnion.Directions.Repeat : InputUnion.Directions.Down, e.Key));
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            Input.Handle(new InputUnion(e.IsRepeat ? InputUnion.Directions.Repeat : InputUnion.Directions.Up, e.Key));
        }

    }
}
