namespace whateverthefuck.src.model
{
    using System;
    using whateverthefuck.src.view;

    /// <summary>
    /// A class representing carthesian coordinate in game.
    /// </summary>
    public class GameCoordinate : Coordinate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCoordinate"/> class.
        /// </summary>
        /// <param name="x">X value of coordinate.</param>
        /// <param name="y">Y value of coordinate.</param>
        public GameCoordinate(float x, float y)
            : base(x, y)
        {
        }

        public static GameCoordinate operator +(GameCoordinate a, GameCoordinate b)
        {
            return new GameCoordinate(a.X + b.X, a.Y + b.Y);
        }

        public static GameCoordinate operator -(GameCoordinate a)
        {
            return new GameCoordinate(-a.X, -a.Y);
        }

        public static GameCoordinate operator -(GameCoordinate a, GameCoordinate b)
        {
            return a + -b;
        }

        /// <summary>
        /// Calculates distance to another GameCoordinate
        /// </summary>
        /// <param name="other">Coordinate to which the distance is to be calculated.</param>
        /// <returns>The distance between this GameCoordinate and the given GameCoordinate.</returns>
        public float Distance(GameCoordinate other)
        {
            var xDistance = this.X - other.X;
            var yDistance = this.Y - other.Y;
            return (float)Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance));
        }
    }
}
