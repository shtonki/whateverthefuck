﻿namespace whateverthefuck.src.view
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using whateverthefuck.src.control;
    using whateverthefuck.src.model;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view.guicomponents;

    public static class GUI
    {
        public static GameState ForceToDrawGameState { get; set; }

        public static List<Drawable> DebugInfo { get; set; } = new List<Drawable>();

        public static List<GUIComponent> GUIComponents { get; set; } = new List<GUIComponent>();

        private static GUIComponent FocusedComponent { get; set; }

        private static GibbWindow Frame { get; set; }

        private static Timer StepTimer { get; set; }

        private static int StepInterval { get; } = 10;

        private static LootPanel LootPanel { get; set; }

        private static InventoryPanel InventoryPanel { get; set; }

        private static EquipmentPanel EquipmentPanel { get; set; }

        private static TargetPanel HeroPanel { get; set; }

        private static TargetPanel TargetPanel { get; set; }

        private static TextPanel ToolTipPanel { get; set; }

        private static DialoguePanel DialoguePanel { get; set; }

        private static CastBar CastBar { get; set; }

        private static SpecializationPanel SpecializationPanel { get; set; }

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

        public static bool HandleGUIInput(InputUnion input)
        {
            if (input.IsMouseMove || input.IsMouseInput)
            {
                if (input.IsMouseInput && input.MouseButton.Value == OpenTK.Input.MouseButton.Right && input.Direction == InputUnion.Directions.Up)
                {
                    HideToolTip();
                }

                var interactedGUIComponent = FirstVisibleGUIComponentAt(input.Location);

                if (interactedGUIComponent != null)
                {
                    if (input.MouseButton == OpenTK.Input.MouseButton.Right && input.Direction == InputUnion.Directions.Down)
                    {
                        if (interactedGUIComponent is ToolTipper)
                        {
                            var tooltip = (interactedGUIComponent as ToolTipper).GetToolTip();
                            ShowToolTip(tooltip, input.Location);
                        }
                    }
                    if (input.IsMouseInput && input.Direction == InputUnion.Directions.Down)
                    {
                        Focus(interactedGUIComponent);
                    }

                    interactedGUIComponent.HandleInput(input);
                    return true;
                }
                else
                {
                    Focus(null);
                    return false;
                }
            }

            if (FocusedComponent != null)
            {
                FocusedComponent.HandleInput(input);
                return true;
            }

            return false;
        }

        public static void LoadGUI()
        {
            LoadInventoryPanel();
        }

        public static void LoadHUD(PC hero)
        {

            SpecializationPanel = new SpecializationPanel(hero.SpecializationTree, new GLCoordinate(1.5f, 1.5f));
            SpecializationPanel.Location = new GLCoordinate(-0.9f, -0.9f);
            GUIComponents.Add(SpecializationPanel);

            LoadAbilityBar(hero);
            SetHeroPanel(hero);
            SetCastBar(hero);
            LoadEquipmentPanel(hero);
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

        public static void ShowToolTip(string tooltip, GLCoordinate location)
        {
            if (ToolTipPanel != null)
            {
                return;
            }
            ToolTipPanel = new TextPanel(tooltip, Color.Black);
            ToolTipPanel.Location = location;
            ToolTipPanel.BackColor = Color.White;
            GUIComponents.Add(ToolTipPanel);
        }

        public static void HideToolTip()
        {
            if (ToolTipPanel != null)
            {
                bool v = GUIComponents.Remove(ToolTipPanel);
                ToolTipPanel = null;
            }
        }

        public static void UpdateInventoryPanel(Inventory inventory)
        {
            InventoryPanel?.Update(inventory);
        }

        public static void ToggleInventoryPanel()
        {
            InventoryPanel.Visible = !InventoryPanel.Visible;
        }

        public static void ToggleEquipmentPanel()
        {
            EquipmentPanel.Visible = !EquipmentPanel.Visible;
        }

        public static void ToggleSpecializationPanel()
        {
            SpecializationPanel.Visible = !SpecializationPanel.Visible;
        }

        public static void SetCastBar(PC hero)
        {
            if (CastBar != null)
            {
                Logging.Log("Adding CastBar with one already there.", Logging.LoggingLevel.Error);
                GUIComponents.Remove(CastBar);
                CastBar = null;
            }

            CastBar castBar = new CastBar(hero);
            castBar.Size = new GLCoordinate(0.9f, 0.2f);
            castBar.Location = new GLCoordinate(-0.4f, -0.5f);

            CastBar = castBar;
            GUIComponents.Add(CastBar);
        }

        public static void SetHeroPanel(PC hero)
        {
            if (HeroPanel != null)
            {
                GUIComponents.Remove(HeroPanel);
                HeroPanel = null;
            }

            TargetPanel heroPanel = new TargetPanel(hero);
            heroPanel.Size = new GLCoordinate(0.6f, 0.25f);
            heroPanel.Location = new GLCoordinate(-0.9f, 0.65f);

            HeroPanel = heroPanel;
            GUIComponents.Add(HeroPanel);
        }

        public static void SetTargetPanel(GameEntity target)
        {
            if (TargetPanel != null)
            {
                GUIComponents.Remove(TargetPanel);
                TargetPanel = null;
                if (target == null)
                {
                    return;
                }
            }

            TargetPanel targetpanel = new TargetPanel(target);
            targetpanel.Size = new GLCoordinate(0.6f, 0.25f);
            targetpanel.Location = new GLCoordinate(-0.2f, 0.65f);

            TargetPanel = targetpanel;
            GUIComponents.Add(TargetPanel);
        }

        public static void AddDamageText(GameCoordinate location, string text, Color color)
        {
            var textPanel = new DamageTextPanel(text, color, location);

            GUIComponents.Add(textPanel);
        }

        public static void ShowDialogue(Dialogue dialogue)
        {
            if (DialoguePanel != null)
            {
                GUIComponents.Remove(DialoguePanel);
                DialoguePanel = null;
            }

            DialoguePanel = new DialoguePanel(dialogue, new GLCoordinate(0.8f, 0.8f));
            GUIComponents.Add(DialoguePanel);
        }

        public static void CleanUp()
        {
            GUIComponents.RemoveAll(gc => gc.DestroyMe);
        }

        public static EntityIdentifier EntityAt(GameCoordinate location)
        {
            if (Frame.EntityDrawingInfoCache == null)
            {
                return null;
            }

            foreach (var ed in Frame.EntityDrawingInfoCache.EntityDrawables)
            {
                if (ed.Contains(location))
                {
                    return ed.Identifier;
                }
            }

            return null;
        }

        private static void StepAllComponents()
        {
            for (int i = 0; i < GUIComponents.Count; i++)
            {
                if (GUIComponents.Count <= i)
                {
                    break;
                }
                var component = GUIComponents[i];

                if (component == null)
                {
                    break;
                }

                component.Step();
            }

        }

        private static void LaunchGameWindow(object o)
        {
            ManualResetEventSlim loadre = (ManualResetEventSlim)o;
            Frame = new GibbWindow(loadre);
            Frame.Run(0, 0);
        }

        private static GUIComponent FirstVisibleGUIComponentAt(GLCoordinate location)
        {
            GUIComponent parent = null;

            foreach (var c in GUIComponents)
            {
                if (c.Contains(location) && c.Visible)
                {
                    parent = c;
                    break;
                }
            }

            if (parent == null)
            {
                return null;
            }

            while (true)
            {
                location -= parent.Location;

                bool cont = false;

                foreach (var child in parent.Children)
                {
                    if (child.Contains(location) && child.Visible)
                    {
                        parent = child;
                        cont = true;
                        break;
                    }
                }

                if (!cont)
                {
                    return parent;
                }
            }
        }

        private static void LoadAbilityBar(PC hero)
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

            var a0 = hero.Abilities.Abilities[0];
            AbilityButton b0 = new AbilityButton(a0);
            abilityBar.AddChild(b0);

            var a1 = hero.Abilities.Abilities[1];
            AbilityButton b1 = new AbilityButton(a1);
            abilityBar.AddChild(b1);

            var a2 = hero.Abilities.Abilities[2];
            AbilityButton b2 = new AbilityButton(a2);
            abilityBar.AddChild(b2);

            hero.OnStep += (entity, gameState) =>
            {
                b0.CooldownPercentage = entity.Abilities.CooldownPercentage(a0);
                b1.CooldownPercentage = entity.Abilities.CooldownPercentage(a1);
                b2.CooldownPercentage = entity.Abilities.CooldownPercentage(a2);
            };

            GUIComponents.Add(abilityBar);
        }

        private static void LoadEquipmentPanel(PC hero)
        {
            EquipmentPanel = new EquipmentPanel(hero.Equipment, new GLCoordinate(0.6f, 0.6f));
            EquipmentPanel.Location = new GLCoordinate(0.1f, 0.1f);
            EquipmentPanel.Visible = false;
            GUIComponents.Add(EquipmentPanel);
        }

        private static void Focus(GUIComponent focused)
        {
            // @add GainFocus and LostFocus events?
            FocusedComponent = focused;
        }

        private static void LoadInventoryPanel()
        {
            InventoryPanel = new InventoryPanel(new GLCoordinate(0.6f, 0.6f));
            InventoryPanel.Location = new GLCoordinate(0.1f, 0.1f);
            GUIComponents.Add(InventoryPanel);
        }
    }
}
