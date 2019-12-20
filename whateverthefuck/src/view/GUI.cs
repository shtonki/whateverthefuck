﻿namespace whateverthefuck.src.view
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using whateverthefuck.src.model;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view.guicomponents;

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

        public static void LoadGUI()
        {
#if true
            return;
            Panel p = new Panel();
            p.Size = new GLCoordinate(0.5f, 0.5f);

            GridLayoutManager glm = new GridLayoutManager();
            glm.Rows = 1;
            glm.Height = 0.1f;
            glm.Width = 0.1f;
            glm.XPadding = 0.05f;
            glm.YPadding = 0.05f;
            p.LayoutManager = glm;

            for (int i = 0; i < 10; i++)
            {
                int v = i;
                Button b = new Button();
                b.BackColor = RNG.RandomColor();
                b.OnMouseButtonDown += (c, input) => Logging.Log(v);
                p.AddChild(b);
            }

            p.Visible = true;

            GUIComponents.Add(p);
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

        public static void LoadAbilityBar(PlayerCharacter hero)
        {
            Panel abilityBar = new Panel();
            abilityBar.Location = new GLCoordinate(-0.9f, -0.9f);
            abilityBar.Size = new GLCoordinate(1.8f, 0.4f);

            GridLayoutManager glm = new GridLayoutManager();
            glm.Height = 0.3f;
            glm.Width = 0.3f;
            glm.XPadding = 0.05f;
            glm.YPadding = 0.05f;
            glm.Rows = 1;
            abilityBar.LayoutManager = glm;

            var a0 = hero.Ability(0);
            AbilityButton b0 = new AbilityButton(a0);
            abilityBar.AddChild(b0);

            var a1 = hero.Ability(1);
            AbilityButton b1 = new AbilityButton(a1);
            abilityBar.AddChild(b1);

            hero.OnStep += e =>
            {
                b0.CooldownPercentage = e.CooldownPercentage(a0);
                b1.CooldownPercentage = e.CooldownPercentage(a1);
            };

            GUIComponents.Add(abilityBar);
        }

        public static GLCoordinate TranslateScreenToGLCoordinates(ScreenCoordinate screenCoordinate)
        {
            var x = ((float)screenCoordinate.X / Frame.ClientSize.Width * 2) - 1;
            var y = -(((float)screenCoordinate.Y / Frame.ClientSize.Height * 2) - 1);

            return new GLCoordinate(x, y);
        }

        public static ScreenCoordinate TranslateGLToScreenCoordinates(GLCoordinate glCoordinate)
        {
            var x = (glCoordinate.X + 1) / 2 * Frame.ClientSize.Width;
            var y = (glCoordinate.Y + 1) / 2 * Frame.ClientSize.Height;

            return new ScreenCoordinate((int)x, (int)y);
        }

        private static void LaunchGameWindow(object o)
        {
            ManualResetEventSlim loadre = (ManualResetEventSlim)o;
            Frame = new GibbWindow(loadre);
            Frame.Run(0, 0);
        }
    }
}
