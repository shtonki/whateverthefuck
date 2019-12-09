using System;
using System.Collections;
using System.Collections.Generic;
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
            var frame = new GibbWindow(loadre);
            frame.Run(0, 0);
        }

        public static void LoadGUI()
        {
            GUIComponents.Add(new Button(new GLCoordinate(-0.8f, -0.8f), new GLCoordinate(0.1f, 0.1f)));

            DraggablePanel f = new DraggablePanel(new GLCoordinate(-0.4f, -0.4f), new GLCoordinate(0.5f, 0.5f));
            Button b1 = new Button(new GLCoordinate(0.1f, 0.1f), new GLCoordinate(0.1f, 0.1f));
            Button b2 = new Button(new GLCoordinate(0.2f, 0.1f), new GLCoordinate(0.1f, 0.1f));
            Button b3 = new Button(new GLCoordinate(0.1f, 0.2f), new GLCoordinate(0.1f, 0.1f));
            Button b4 = new Button(new GLCoordinate(0.2f, 0.2f), new GLCoordinate(0.1f, 0.1f));
            f.Add(b1, b2, b3, b4);

            GUIComponents.Add(f);
        }
    }
}
