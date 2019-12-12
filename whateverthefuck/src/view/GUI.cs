using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.view.guicomponents;

namespace whateverthefuck.src.view
{
    public static class GUI
    {
        public static Camera Camera { get; set; }

        public static GameState ForceToDrawGameState { get; set; }
        public static List<Drawable> DebugInfo { get; set; } = new List<Drawable>();
        public static List<GUIComponent> GUIComponents { get; set; } = new List<GUIComponent>();
        private static GibbWindow Frame { get; set; }
        /// <summary>
        /// Creates a GibbWindow on a new thread and wait for the OnLoad event
        /// of said window to be called. Roughly speaking.
        /// </summary>
        public static void CreateGameWindow()
        {
            Camera = new StaticCamera(new GameCoordinate(0, 0));

            ManualResetEventSlim loadre = new ManualResetEventSlim();
            Thread t = new Thread(LaunchGameWindow);
            t.Start(loadre);
            loadre.Wait();
        }

        public static IEnumerable<Drawable> GetAllDrawables()
        {
            IEnumerable<Drawable> entities;
            if (ForceToDrawGameState != null)
            {
                entities = ForceToDrawGameState.AllEntities;
            }
            else
            {
                entities = Program.GameStateManager.GameState.AllEntities;
            }


            return entities
                .Concat(DebugInfo)
                ;
        }

        private static void LaunchGameWindow(object o)
        {
            ManualResetEventSlim loadre = (ManualResetEventSlim)o;
            Frame = new GibbWindow(loadre);
            Frame.Run(0, 0);
        }

        public static void LoadGUI()
        {
#if true
            Panel p = new Panel();
            p.Location = new GLCoordinate(0.45f, 0.45f);
            p.Size = new GLCoordinate(0.2f, 0.2f);
            p.BackColor = Color.Pink;
            p.Add(new Button(new GLCoordinate(0.05f, 0.05f), new GLCoordinate(0.1f, 0.1f)));
            GUIComponents.Add(p);

            DraggablePanel dp = new DraggablePanel(new GLCoordinate(-0.05f, -0.95f), new GLCoordinate(1, 1));
            dp.BackColor = Color.Yellow;
            GUIComponents.Add(dp);

            //GUIComponents.Add(dp);
#else
            //GUIComponents.Add(new Button(new GLCoordinate(-0.8f, -0.8f), new GLCoordinate(0.1f, 0.1f)));

            DraggablePanel f = new DraggablePanel(new GLCoordinate(-0.1f, -0.1f), new GLCoordinate(0.5f, 0.5f));
            //f.AddMenuBar();
            Button b1 = new Button(new GLCoordinate(0.1f, 0.1f), new GLCoordinate(0.1f, 0.1f));
            Button b2 = new Button(new GLCoordinate(0.3f, 0.1f), new GLCoordinate(0.1f, 0.1f));
            Button b3 = new Button(new GLCoordinate(0.1f, 0.3f), new GLCoordinate(0.1f, 0.1f));
            Button b4 = new Button(new GLCoordinate(0.3f, 0.3f), new GLCoordinate(0.1f, 0.1f));
            f.Add(b1, b2, b3, b4);

            GUIComponents.Add(f);
#endif
        }

        public static GLCoordinate TranslateScreenToGLCoordinates(ScreenCoordinate screenCoordinate)
        {
            var X = ((float)screenCoordinate.X / Frame.ClientSize.Width) * 2 - 1;
            var Y = -(((float)screenCoordinate.Y / Frame.ClientSize.Height) * 2 - 1);

            return new GLCoordinate(X, Y);
        }

        public static ScreenCoordinate TranslateGLToScreenCoordinates(GLCoordinate glCoordinate)
        {
            var X = (glCoordinate.X + 1) / 2 * Frame.ClientSize.Width;
            var Y = ((glCoordinate.Y + 1) / 2 * Frame.ClientSize.Height);

            return new ScreenCoordinate((int)X, (int)Y);
        }

#if false
        // jesus fuck
        public static GLCoordinate TranslateScreenToGLCoordinates(ScreenCoordinate sc)
        {
            var correctish = SToGL(sc);
            return new GLCoordinate(sc.X * 2.0f / Frame.ClientSize.Width - 1, -sc.Y * 2.0f / Frame.ClientSize.Height - 1);
        }

        public static ScreenCoordinate TranslateGLToScreenCoordinates(GLCoordinate gl)
        {
            int x = (int) ((gl.X + 1) * Frame.ClientSize.Width / 2);
            int y = (int) ((gl.Y + 1) * Frame.ClientSize.Height / 2); 

            return new ScreenCoordinate(x, y);
        }
#endif
    }
}
