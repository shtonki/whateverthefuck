using OpenTK.Input;
using System;
using whateverthefuck.src.view;

namespace whateverthefuck.src.control
{
    public class InputUnion : IEquatable<InputUnion>
    {
        public enum Directions { Undefined, Up, Down, Repeat };
        public Directions Direction { get; }

        public Key? Key { get; }
        public bool IsKeyboardInput => Key.HasValue;

        public MouseButton? MouseButton { get; }
        public bool IsMouseInput => MouseButton.HasValue;

        public GLCoordinate Location { get; }

        public InputUnion(Directions direction, MouseButton mouseButton, ScreenCoordinate location)
        {
            Direction = direction;
            MouseButton = mouseButton;
            Location = GUI.TranslateScreenToGLCoordinates(location);
        }

        public InputUnion(Directions direction, Key key)
        {
            Direction = direction;
            Key = key;
        }

        public bool Equals(InputUnion other)
        {
            return Direction == other.Direction &&
                ((IsKeyboardInput && other.IsKeyboardInput && Key.Value == other.Key.Value) ||
                 (IsMouseInput && other.IsMouseInput && MouseButton.Value == other.MouseButton.Value));
        }
    }

}