using OpenTK.Input;
using System;
using whateverthefuck.src.view;

namespace whateverthefuck.src.control
{

    /// <summary>
    /// This class is fucked.
    /// It is used to represent any possible input action the user may take.
    /// This includes mouse moving, buttons being pressed, mouse buttons being pressed, scroll wheel being spinned.
    /// It saved everything in one uninherited class for "ease of use".
    /// Have fun.
    /// </summary>
    public class InputUnion : IEquatable<InputUnion>
    {
        public enum Directions { Undefined, Up, Down };
        public Directions Direction { get; }

        public Key? Key { get; }
        public bool IsKeyboardInput => Key.HasValue;

        public MouseButton? MouseButton { get; }
        public bool IsMouseInput => MouseButton.HasValue;

        public GLCoordinate Location { get; }
        public GLCoordinate PreviousLocation { get; }
        public bool IsMouseMove => PreviousLocation != null;


        public InputUnion(ScreenCoordinate beforeMove, ScreenCoordinate afterMove)
        {
            Location = GUI.TranslateScreenToGLCoordinates(afterMove);
            PreviousLocation = GUI.TranslateScreenToGLCoordinates(beforeMove);
        }

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