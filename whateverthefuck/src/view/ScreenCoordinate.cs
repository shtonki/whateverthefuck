namespace whateverthefuck.src.view
{
    public class ScreenCoordinate : Coordinate
    {
        public ScreenCoordinate(int x, int y)
            : base(x, y)
        {
        }

        public new int X => (int)base.X;

        public new int Y => (int)base.Y;
    }
}
