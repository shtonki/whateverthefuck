using System.Collections.Generic;
using OpenTK.Input;
using System;
using whateverthefuck.src.view;

namespace whateverthefuck.src.control
{
    static class Input
    {
        private static List<HotkeyMapping> HotkeyMappings = new List<HotkeyMapping>();
        
        static Input()
        {
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Down, Key.W), GameAction.HeroWalkUpwards));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Up, Key.W), GameAction.HeroWalkUpwardsStop));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Down, Key.S), GameAction.HeroWalkDownwards));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Up, Key.S), GameAction.HeroWalkDownwardsStop));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Down, Key.A), GameAction.HeroWalkLeftwards));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Up, Key.A), GameAction.HeroWalkLeftwardsStop));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Down, Key.D), GameAction.HeroWalkRightwards));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Up, Key.D), GameAction.HeroWalkRightwardsStop));

            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Down, MouseButton.Middle), GameAction.CameraZoomIn));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Up, MouseButton.Middle), GameAction.CameraZoomOut));
        }


        public static void Handle(InputUnion input)
        {
            foreach (var hotkey in HotkeyMappings)
            {
                if (input.Equals(hotkey.Hotkey))
                {
                    Program.GameStateManager.ActivateAction(hotkey.GameAction);
                }
            }
        }
    }
}
