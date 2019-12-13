using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

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

        public GLCoordinate ToGLCoordinate()
        {
            if (this is GLCoordinate) { return this as GLCoordinate; }
            if (this is GameCoordinate) { return GUI.Camera.GameToGLCoordinate(this as GameCoordinate) as GLCoordinate; }
            if (this is ScreenCoordinate) { return GUI.TranslateScreenToGLCoordinates(this as ScreenCoordinate) as GLCoordinate; }

            throw new NotImplementedException();
        }

        public static float AngleBetweenCoordinates(Coordinate c1, Coordinate c2)
        {
            if (c1 == null || c2 == null)
            {
                return 0;
            }

            var deltaX = c1.X - c2.X;
            var deltaY = c1.Y - c2.Y;
            var alpha = Math.Atan2(deltaY, -deltaX) + (Math.PI/2);
            return (float)alpha;
        }

        public static float DistanceBetweenCoordinates(Coordinate c1, Coordinate c2)
        {
            if (c1 == null || c2 == null)
            {
                return 0;
            }

            var deltaX = c1.X - c2.X;
            var deltaY = c1.Y - c2.Y;
            return (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        public override string ToString()
        {
            return String.Format("[{0:0.00} {1:0.00}]", X, Y);
        }
    }
}
