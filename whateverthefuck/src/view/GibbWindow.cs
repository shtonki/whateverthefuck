namespace whateverthefuck.src.view
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;
    using whateverthefuck.src.control;
    using whateverthefuck.src.util;

    internal class GibbWindow : GameWindow
    {
        private ManualResetEventSlim loadResetEvent;

        private int lastSecond;
        private int renderCounter;

        public GibbWindow(ManualResetEventSlim loadResetEvent)
            : base(600, 600, new GraphicsMode(32, 24, 0, 32), "GibbWindow")
        {
            this.ClientSize = new Size(600, 600);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            this.loadResetEvent = loadResetEvent;
        }

        public static Matrix4 ProjectionMatrix { get; private set; }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, this.Width, this.Height);
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(ClientRectangle.X, ClientRectangle.Width, ClientRectangle.Y, ClientRectangle.Height, -1.0f, 1.0f);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ImageLoader.Init();
            Logging.Log("ImageLoader initialized.");

            FontLoader.LoadFonts();
            Logging.Log("Fonts = loaded");

            if (this.loadResetEvent != null)
            {
                this.loadResetEvent.Set();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            var newSecond = DateTime.Now.Second;

            if (newSecond == this.lastSecond)
            {
                this.renderCounter++;
            }
            else
            {
                this.lastSecond = newSecond;
                this.Title = this.renderCounter.ToString();
                this.renderCounter = 0;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            FontLoader.Drawing.ProjectionMatrix = GibbWindow.ProjectionMatrix;
            FontLoader.Drawing.DrawingPrimitives.Clear();

            GL.UseProgram(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ClearColor(Color.Fuchsia);
            GL.PushMatrix();

            var drawAdapter = new DrawAdapter();

            GUI.Camera.Lock();

            // Draw GUI components
            foreach (var guiComponent in GUI.GUIComponents.ToList())
            {
                guiComponent.Draw(drawAdapter);
            }

            // Take account of Camera zoom
            if (GUI.Camera != null)
            {
                // GL.Scale(GUI.Camera.Zoom.CurrentZoom, GUI.Camera.Zoom.CurrentZoom, 0);
            }

            // Draw entities with account to Camera location and zoom
            // @Reconsider: refactor GetAllDrawables to GetAllEntities
            // @High cost operation warning: Order by will eat many cycles, be very careful
            foreach (var drawable in GUI.GetAllDrawables().OrderBy(d => d.Height))
            {
                drawable.Draw(drawAdapter);
            }

            GUI.Camera.Unlock();

            FontLoader.Drawing.RefreshBuffers();
            FontLoader.Drawing.Draw();

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
            var mouseMovedInput = InputUnion.MakeMouseMoveInput(new ScreenCoordinate(e.X, e.Y), new ScreenCoordinate(e.X - e.XDelta, e.Y - e.YDelta));
            Program.GameStateManager.HandleInput(mouseMovedInput);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            InputUnion mouseInput = InputUnion.MakeMouseButtonInput(InputUnion.Directions.Down, e.Button, new ScreenCoordinate(e.X, e.Y));
            Program.GameStateManager.HandleInput(mouseInput);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            InputUnion mouseInput = InputUnion.MakeMouseButtonInput(InputUnion.Directions.Up, e.Button, new ScreenCoordinate(e.X, e.Y));
            Program.GameStateManager.HandleInput(mouseInput);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            InputUnion keyboardInput = InputUnion.MakeKeyboardInput(InputUnion.Directions.Down, e.Key);
            Program.GameStateManager.HandleInput(keyboardInput);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            InputUnion keyboardInput = InputUnion.MakeKeyboardInput(InputUnion.Directions.Up, e.Key);
            Program.GameStateManager.HandleInput(keyboardInput);
        }
    }
}
