namespace whateverthefuck.src.view
{
    public class GLCoordinate : Coordinate
    {
        public GLCoordinate(float x, float y)
            : base(x, y)
        {
        }


        public static GLCoordinate operator +(GLCoordinate a, GLCoordinate b)
        {
            return new GLCoordinate(a.X + b.X, a.Y + b.Y);
        }

        public static GLCoordinate operator -(GLCoordinate a)
        {
            return new GLCoordinate(-a.X, -a.Y);
        }

        public static GLCoordinate operator -(GLCoordinate a, GLCoordinate b)
        {
            return a + -b;
        }
    }
}
