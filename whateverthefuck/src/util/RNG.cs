using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.view;

namespace whateverthefuck.src.util
{
    public static class RNG
    {
        static Random r = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Return a random point within radius.
        /// </summary>
        /// <param name="Radius"></param>
        /// <returns></returns>
        public static GameCoordinate RandomPointWithinCircle(GLCoordinate Radius)
        {
            var re = Radius.X * Math.Sqrt(r.NextDouble());
            var theta = r.NextDouble() * 2 * (float)Math.PI;

            return new GameCoordinate((float)(re * (float)Math.Cos(theta)), (float)(re * (float)Math.Sin(theta)));
        }

        /// <summary>
        /// Returns a random point within a circle around a given location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="circleRadius"></param>
        /// <returns></returns>
        public static GameCoordinate RandomPointWithinCircleRelativeToLocation(GameCoordinate location, GLCoordinate circleRadius)
        {
            var pointInCircle = RandomPointWithinCircle(circleRadius);
            return new GameCoordinate(location.X + pointInCircle.X, location.Y + pointInCircle.Y);
        }

        /// <summary>
        /// Returns true x out of y times.
        /// For example x = 5, y = 100 would return true 5 in 100 times.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool XTimesInY(int x, int y)
        {
            return r.Next() % y < x;
        }

        /// <summary>
        /// Returns an integer between a(including) and b(excluding)
        /// </summary>
        /// <param name="a">min</param>
        /// <param name="b">max</param>
        /// <returns> a <= n < b </returns>
        public static int IntegerBetween(int a, int b)
        {
            return r.Next(a, b);
        }

        /// <summary>
        /// Returns a float between [0, 1)
        /// </summary>
        /// <returns></returns>
        public static float BetweenZeroAndOne()
        {
            return (float)r.NextDouble();
        }

        /// <summary>
        /// Returns either -1 or 1.
        /// </summary>
        /// <returns></returns>
        public static int NegativeOrPositiveOne()
        {
            return r.Next() % 2 == 0 ? 1 : -1;
        }

        /// <summary>
        /// Returns a random location within a square defined from two points.
        /// For now unexpected stuff can happen if terminus is bigger than origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="terminus"></param>
        /// <returns></returns>
        public static GameCoordinate RandomLocationWithinSquare(GameCoordinate origin, GameCoordinate terminus)
        {
            var rngLoc = new GameCoordinate((float)r.NextDouble() % (terminus.X - origin.X) + origin.X, (float)r.NextDouble() % (terminus.Y - origin.Y) + origin.Y);
            return rngLoc;
        }

        /// <summary>
        /// Returns a random element in the provided list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomElement<T>(List<T> list)
        {
            return list.ElementAt(RNG.IntegerBetween(0, list.Count()));
        }
    }
}
