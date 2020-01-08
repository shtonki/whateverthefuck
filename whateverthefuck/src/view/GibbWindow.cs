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
    using QuickFont;
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

            FontDrawing = new QFontDrawing();

            this.loadResetEvent = loadResetEvent;
        }

        public static Matrix4 ProjectionMatrix { get; private set; }

        public static QFontDrawing FontDrawing { get; private set; }

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

            this.UpdateFPSCounter();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Fuchsia);

            var drawAdapter = new DrawAdapter(FontDrawing);

            drawAdapter.PushMatrix();

            FontDrawing.ProjectionMatrix = GibbWindow.ProjectionMatrix;
          //FontDrawing.DrawingPrimitives.Clear();

            GL.UseProgram(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Draw GUI components
            foreach (var guiComponent in GUI.GUIComponents.ToList())
            {
                guiComponent.Draw(drawAdapter);
            }
#if false
            // Take account of Camera zoom
            if (GUI.Camera != null)
            {
                // GL.Scale(GUI.Camera.Zoom.CurrentZoom, GUI.Camera.Zoom.CurrentZoom, 0);
            }
#endif

            var entityDrawing = Program.GameStateManager.GameState.EntityDrawingInfo;

            if (entityDrawing != null)
            {
                drawAdapter.PushMatrix();

                drawAdapter.Translate(-entityDrawing.Camera.Location.X, -entityDrawing.Camera.Location.Y);

                foreach (var ed in entityDrawing.EntityDrawables)
                {
                    ed.Draw(drawAdapter);
                }

                drawAdapter.PopMatrix();
            }

            FontDrawing.RefreshBuffers();
            FontDrawing.Draw();

            this.SwapBuffers();
            drawAdapter.PopMatrix();

            if (drawAdapter.MatrixCount != 0)
            {
                Logging.Log("Unbalanced Push/Pop of GL matricies", Logging.LoggingLevel.Warning);
            }
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
#if false
            var mouseMovedInput = InputUnion.MakeMouseMoveInput(new ScreenCoordinate(e.X, e.Y), new ScreenCoordinate(e.X - e.XDelta, e.Y - e.YDelta));
            Program.GameStateManager.HandleInput(mouseMovedInput);
#endif
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

        private void UpdateFPSCounter()
        {
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
        }
    }
}
