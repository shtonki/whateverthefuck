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
        public static List<Drawable> Extras { get; set; } = new List<Drawable>();
        public static List<GUIComponent> GUIComponents { get; set; } = new List<GUIComponent>();

        /// <summary>
        /// Creates a GibbWindow on a new thread and wait for the OnLoad event
        /// of said window to be called. Roughly speaking.
        /// </summary>
        public static void CreateGameWindow()
        {
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
                .Concat(Extras)
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
            GUIComponents.Add(new Button(new GLCoordinate(-1f, -1f), new GLCoordinate(0.1f, 0.1f)));
        }
    }
}
