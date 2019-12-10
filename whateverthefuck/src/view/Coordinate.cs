using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.view
{
    public abstract class Coordinate
    {
        public float X { get; set; }
        public float Y { get; set; }

        protected Coordinate(float x, float y)
        {
            (X, Y) = (x, y);
        }

        public static float AngleBetweenCoordinates(Coordinate c1, Coordinate c2)
        {
            if (c1 == null || c2 == null) return 0;
            var deltaX = c1.X - c2.X;
            var deltaY = c1.Y - c2.Y;
            var alpha = Math.Atan2(deltaY, -deltaX) + Math.PI/2;
            return (float)alpha;
        }

        public static float DistanceBetweenCoordinates(Coordinate c1, Coordinate c2)
        {
            if (c1 == null || c2 == null) return 0;

            var deltaX = c1.X - c2.X;
            var deltaY = c1.Y - c2.Y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

    }
}
