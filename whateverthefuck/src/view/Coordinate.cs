namespace whateverthefuck.src.view
{
    using System;
    using whateverthefuck.src.model;

    public abstract class Coordinate
    {
        protected Coordinate(float x, float y)
        {
            (this.X, this.Y) = (x, y);
        }

        public float X { get; set; }

        public float Y { get; set; }

        public static float AngleBetweenCoordinates(Coordinate c1, Coordinate c2)
        {
            if (c1 == null || c2 == null)
            {
                return 0;
            }

            var deltaX = c1.X - c2.X;
            var deltaY = c1.Y - c2.Y;
            var alpha = Math.Atan2(deltaY, -deltaX) + (Math.PI / 2);
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

        public GLCoordinate ToGLCoordinate()
        {
            if (this is GLCoordinate) { return this as GLCoordinate; }
            if (this is GameCoordinate) { return GUI.Camera.GameToGLCoordinate(this as GameCoordinate) as GLCoordinate; }
            if (this is ScreenCoordinate) { return GUI.TranslateScreenToGLCoordinates(this as ScreenCoordinate) as GLCoordinate; }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("[{0:0.00} {1:0.00}]", this.X, this.Y);
        }
    }
}
