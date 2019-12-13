namespace whateverthefuck.src.control
{
    using System.Collections.Generic;
    using OpenTK.Input;

    /// <summary>
    /// A collection of HotkeyMappings.
    /// </summary>
    internal static class Hotkeys
    {
        static Hotkeys()
        {
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Down, Key.W), GameAction.HeroWalkUpwards));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Up, Key.W), GameAction.HeroWalkUpwardsStop));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Down, Key.S), GameAction.HeroWalkDownwards));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Up, Key.S), GameAction.HeroWalkDownwardsStop));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Down, Key.A), GameAction.HeroWalkLeftwards));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Up, Key.A), GameAction.HeroWalkLeftwardsStop));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Down, Key.D), GameAction.HeroWalkRightwards));
            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Up, Key.D), GameAction.HeroWalkRightwardsStop));

            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Down, Key.Space), GameAction.CastAbility1));

            HotkeyMappings.Add(new HotkeyMapping(InputUnion.MakeKeyboardInput(InputUnion.Directions.Up, Key.Tab), GameAction.TogglePanel));
        }

        private static List<HotkeyMapping> HotkeyMappings { get; } = new List<HotkeyMapping>();

        /// <summary>
        /// Looks up an input and returns the corresponding GameAction.
        /// </summary>
        /// <param name="input">The InputUnion to lookup.</param>
        /// <returns>The corresponding GameAction if input is mapped, GameAction.Undefined otherwise.</returns>
        public static GameAction LookupHotkey(InputUnion input)
        {
            foreach (var hotkey in HotkeyMappings)
            {
                if (input.Equals(hotkey.Hotkey))
                {
                    return hotkey.GameAction;
                }
            }

            return GameAction.Undefined;
        }
    }


    /// <summary>
    /// A class which maps an InputUnion to a GameAction.
    /// </summary>
    internal class HotkeyMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyMapping"/> class mapping the given InputUnion to a GameAction.
        /// </summary>
        /// <param name="hotkey">The InputUnion mapped from.</param>
        /// <param name="gameAction">The GameAction mapped to.</param>
        public HotkeyMapping(InputUnion hotkey, GameAction gameAction)
        {
            this.Hotkey = hotkey;
            this.GameAction = gameAction;
        }

        /// <summary>
        /// Gets the InputUnion mapped from.
        /// </summary>
        public InputUnion Hotkey { get; }

        /// <summary>
        /// Gets the GameAction mapped to.
        /// </summary>
        public GameAction GameAction { get; }

    }
}
