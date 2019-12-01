using OpenTK.Input;

namespace whateverthefuck.src.control
{
    /// <summary>
    /// A class which maps an InputUnion to a GameAction
    /// </summary>
    class HotkeyMapping
    {
        public InputUnion Hotkey { get; }
        public GameAction GameAction { get; }

        public HotkeyMapping(InputUnion hotkey, GameAction gameAction)
        {
            this.Hotkey = hotkey;
            this.GameAction = gameAction;
        }
    }
}
