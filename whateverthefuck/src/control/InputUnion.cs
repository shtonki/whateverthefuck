namespace whateverthefuck.src.control
{
    using System;
    using OpenTK.Input;
    using whateverthefuck.src.view;

    /// <summary>
    /// This class is fucked.
    /// It is used to represent any possible input action the user may take.
    /// This includes mouse moving, buttons being pressed, mouse buttons being pressed, scroll wheel being spinned.
    /// It saved everything in one uninherited class for "ease of use".
    /// Have fun.
    /// </summary>
    public class InputUnion : IEquatable<InputUnion>
    {
        private InputUnion(ScreenCoordinate beforeMove, ScreenCoordinate afterMove)
        {
            this.Location = GUI.ScreenToGLCoordinates(afterMove);
            this.PreviousLocation = GUI.ScreenToGLCoordinates(beforeMove);
        }

        private InputUnion(Directions direction, MouseButton mouseButton, ScreenCoordinate location)
        {
            this.Direction = direction;
            this.MouseButton = mouseButton;
            this.Location = GUI.ScreenToGLCoordinates(location);
        }

        private InputUnion(Directions direction, Key key)
        {
            this.Direction = direction;
            this.Key = key;
        }

        public enum Directions
        {
            Undefined,
            Up,
            Down,
        }

        public Directions Direction { get; }

        /// <summary>
        /// Gets the Key being pressedor, or null if the InputUnion is not a keyboard input.
        /// </summary>
        public Key? Key { get; }

        public bool IsKeyboardInput => this.Key.HasValue;

        /// <summary>
        /// Gets the MouseButton being pressed, or null if the InputUnion is not a mouse input.
        /// </summary>
        public MouseButton? MouseButton { get; }

        public bool IsMouseInput => this.MouseButton.HasValue;

        /// <summary>
        /// Gets the location of the mouse event.
        /// </summary>
        public GLCoordinate Location { get; }

        /// <summary>
        /// Gets the location of where the mouse moved from, or null if the InputUnion is not a mouse move event.
        /// </summary>
        public GLCoordinate PreviousLocation { get; }

        public bool IsMouseMove => this.PreviousLocation != null;

        public static InputUnion MakeMouseMoveInput(ScreenCoordinate beforeMove, ScreenCoordinate afterMove)
        {
            return new InputUnion(beforeMove, afterMove);
        }

        public static InputUnion MakeKeyboardInput(Directions direction, Key key)
        {
            return new InputUnion(direction, key);
        }

        public static InputUnion MakeMouseButtonInput(Directions direction, MouseButton button, ScreenCoordinate location)
        {
            return new InputUnion(direction, button, location);
        }

        /// <summary>
        /// Checks the equality of two InputUnions. They are equal if and only if the represent the same keyboard key or mouse button being pressed in the same direction.
        /// </summary>
        /// <param name="other">The InputUnion with which to check for equality.</param>
        /// <returns>True if the compared InputEvent represents the same button or key being pressed, false otherwise.</returns>
        public bool Equals(InputUnion other)
        {
            return this.Direction == other.Direction &&
                ((this.IsKeyboardInput && other.IsKeyboardInput && this.Key.Value == other.Key.Value) ||
                 (this.IsMouseInput && other.IsMouseInput && this.MouseButton.Value == other.MouseButton.Value));
        }
    }
}