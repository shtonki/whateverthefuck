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
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Down, Key.W), GameAction.HeroWalk));
            HotkeyMappings.Add(new HotkeyMapping(new InputUnion(InputUnion.Directions.Up, Key.W), GameAction.HeroStopWalk));
        }

        public static void Handle(InputUnion input)
        {
            foreach (var hotkey in HotkeyMappings)
            {
                if (input.Equals(hotkey.Hotkey))
                {
                    Program.GameState.ActivateAction(hotkey.GameAction);
                }
            }
        }
    }
}
