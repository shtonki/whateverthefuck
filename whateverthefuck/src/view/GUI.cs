namespace whateverthefuck.src.view
{
    using System;
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
        //public static Camera Camera { get; set; }

        public static GameState ForceToDrawGameState { get; set; }

        public static List<Drawable> DebugInfo { get; set; } = new List<Drawable>();

        public static List<GUIComponent> GUIComponents { get; set; } = new List<GUIComponent>();

        private static GibbWindow Frame { get; set; }

        private static Timer StepTimer { get; set; }

        private static int StepInterval { get; } = 10;

        private static LootPanel LootPanel { get; set; }

        private static InventoryPanel InventoryPanel { get; set; } = new InventoryPanel();

        /// <summary>
        /// Creates a GibbWindow on a new thread and wait for the OnLoad event
        /// of said window to be called. Roughly speaking.
        /// </summary>
        public static void CreateGameWindow()
        {
            StepTimer = new Timer(_ => StepAllComponents(), null, 0, StepInterval);

            ManualResetEventSlim loadre = new ManualResetEventSlim();
            Thread t = new Thread(LaunchGameWindow);
            t.Start(loadre);
            loadre.Wait();
        }

        public static IEnumerable<Drawable> GetAllDrawables()
        {
            throw new NotImplementedException();
#if false
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
#endif
        }

        public static void LoadGUI()
        {
            GUIComponents.Add(InventoryPanel);
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

            var a2 = hero.Ability(1);
            AbilityButton b2 = new AbilityButton(a2);
            abilityBar.AddChild(b2);

            hero.OnStep += (entity, gameState) =>
            {
                b0.CooldownPercentage = entity.CooldownPercentage(a0);
                b1.CooldownPercentage = entity.CooldownPercentage(a1);
                b2.CooldownPercentage = entity.CooldownPercentage(a2);
            };

            GUIComponents.Add(abilityBar);
        }

        public static void ShowLoot(Lootable lootee)
        {
            LootPanel = new LootPanel(new GLCoordinate(0.85f, 0.85f / 6), lootee);
            LootPanel.Location = new GLCoordinate(-0.425f, 0.1f);

            GUIComponents.Add(LootPanel);
        }

        public static void CloseLootPanel()
        {
            GUIComponents.Remove(LootPanel);
        }

        public static GLCoordinate ScreenToGLCoordinates(ScreenCoordinate screenCoordinate)
        {
            var x = ((float)screenCoordinate.X / Frame.ClientSize.Width * 2) - 1;
            var y = -(((float)screenCoordinate.Y / Frame.ClientSize.Height * 2) - 1);

            return new GLCoordinate(x, y);
        }

        public static ScreenCoordinate GLToScreenCoordinates(GLCoordinate glCoordinate)
        {
            var x = (glCoordinate.X + 1) / 2 * Frame.ClientSize.Width;
            var y = (glCoordinate.Y + 1) / 2 * Frame.ClientSize.Height;

            return new ScreenCoordinate((int)x, (int)y);
        }

        public static void UpdateInventoryPanel(Inventory inventory)
        {
            InventoryPanel.Update(inventory);
        }

        public static void ToggleInventoryPanel()
        {
            InventoryPanel.Visible = !InventoryPanel.Visible;
        }

        public static void AddDamageText(GameCoordinate location, string text, Color color)
        {
            var textPanel = new DamageTextPanel(text, color, location);

            GUIComponents.Add(textPanel);
        }
#if false
        public static GLCoordinate GameToGLCoordinate(GameCoordinate gameCoordinate)
        {
            var x = gameCoordinate.X - Camera.Location.X;
            var y = gameCoordinate.Y - Camera.Location.Y;
            return new GLCoordinate(x, y);
        }

        public static GameCoordinate GLToGameCoordinate(GLCoordinate glCoordinate)
        {
            return new GameCoordinate(
                (glCoordinate.X / Camera.Zoom.CurrentZoom) + Camera.Location.X,
                (glCoordinate.Y / Camera.Zoom.CurrentZoom) + Camera.Location.Y);
        }
#endif
        private static void StepAllComponents()
        {
            foreach (var component in GUIComponents)
            {
                component.Step();
            }
        }

        private static void LaunchGameWindow(object o)
        {
            ManualResetEventSlim loadre = (ManualResetEventSlim)o;
            Frame = new GibbWindow(loadre);
            Frame.Run(0, 0);
        }
    }
}
