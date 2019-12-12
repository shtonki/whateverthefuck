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
using whateverthefuck.src.util;

namespace whateverthefuck.src.view
{
    class GibbWindow : GameWindow
    {

        private ManualResetEventSlim LoadResetEvent;

        public GibbWindow(ManualResetEventSlim loadResetEvent) : base(600, 600, new GraphicsMode(32, 24, 0, 32), "GibbWindow")
        {
            ClientSize = new Size(600, 600);

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

        private int LastSecond;
        private int RenderCounter;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            var newSecond = DateTime.Now.Second;

            if (newSecond == LastSecond)
            {
                RenderCounter++;
            }
            else
            {
                LastSecond = newSecond;
                Title = RenderCounter.ToString();
                RenderCounter = 0;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.ClearColor(Color.Fuchsia);
            GL.PushMatrix();


            var drawAdapter = new DrawAdapter();

            // Draw GUI components
            foreach (var guiComponent in GUI.GUIComponents)
            {
                guiComponent.Draw(drawAdapter);
            }

            // Take account of Camera location and zoom
            if (GUI.Camera != null)
            {
                //GL.Scale(GUI.Camera.Zoom.CurrentZoom, GUI.Camera.Zoom.CurrentZoom, 0);
                //GL.Translate(-GUI.Camera.Location.X, -GUI.Camera.Location.Y, 0);
            }

            // Draw entities with account to Camera location and zoom
            // @Reconsider: refactor GetAllDrawables to GetAllEntities 
            // @High cost operation warning: Order by will eat many cycles, be very careful
            foreach (var drawable in GUI.GetAllDrawables().OrderBy(d => d.Height))
            {
                drawable.Draw(drawAdapter);
            }


            this.SwapBuffers();
            GL.PopMatrix();
        }



        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            throw new NotImplementedException();
#if false
            if (e.Delta < 0)
            {
                Program.GameStateManager.HandleMouseScroll(new InputUnion(InputUnion.Directions.Down, MouseButton.Middle), new ScreenCoordinate(e.X, e.Y));
            }
            else
            {
                Program.GameStateManager.HandleMouseScroll(new InputUnion(InputUnion.Directions.Up, MouseButton.Middle), new ScreenCoordinate(e.X, e.Y));
            }
#endif
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            var mouseMovedInput = new InputUnion(new ScreenCoordinate(e.X, e.Y), new ScreenCoordinate(e.X - e.XDelta, e.Y - e.YDelta));
            Program.GameStateManager.HandleInput(mouseMovedInput);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            InputUnion mouseInput = new InputUnion(InputUnion.Directions.Down, e.Button, new ScreenCoordinate(e.X, e.Y));
            Program.GameStateManager.HandleInput(mouseInput);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            InputUnion mouseInput = new InputUnion(InputUnion.Directions.Up, e.Button, new ScreenCoordinate(e.X, e.Y));
            Program.GameStateManager.HandleInput(mouseInput);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            InputUnion keyboardInput = new InputUnion(InputUnion.Directions.Down, e.Key);
            Program.GameStateManager.HandleInput(keyboardInput);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            InputUnion keyboardInput = new InputUnion(InputUnion.Directions.Up, e.Key);
            Program.GameStateManager.HandleInput(keyboardInput);
        }

    }
}
