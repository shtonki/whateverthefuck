using OpenTK.Input;
using System;

namespace whateverthefuck.src.control
{
    class InputUnion : IEquatable<InputUnion>
    {
        public enum Directions { Undefined, Up, Down, Repeat };
        public Directions Direction { get; }

        public Key? Key { get; }
        public bool IsKeyboardInput => Key.HasValue;

        public MouseButton? MouseButton { get; }
        public bool IsMouseInput => MouseButton.HasValue;

        public InputUnion(Directions direction, MouseButton mouseButton)
        {
            Direction = direction;
            MouseButton = mouseButton;
        }

        public InputUnion(Directions direction, Key key)
        {
            Direction = direction;
            Key = key;
        }

#if false
        public override string ToString()
        {
            if (IsKeyboardInput)
            {
                return KeyboardArgs.Key.ToString();
            }
            if (IsGamePadInput)
            {
                return GamePadButtonArgs.ToString();
            }
            if (IsMouseInput)
            {
                return MouseButtonArgs.ToString();
            }
            return "Failed to stringify InputUnion";
        }

#endif
        public bool Equals(InputUnion other)
        {
            return Direction == other.Direction &&
                ((IsKeyboardInput && other.IsKeyboardInput && Key.Value == other.Key.Value) ||
                 (IsMouseInput && other.IsMouseInput && MouseButton.Value == other.MouseButton.Value));
        }
    }

}