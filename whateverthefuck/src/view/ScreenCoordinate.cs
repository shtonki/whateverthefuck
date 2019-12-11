using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.view
{
    public class ScreenCoordinate : Coordinate
    {
        public int X => (int)base.X;
        public int Y => (int)base.Y;

        public ScreenCoordinate(int x, int y) : base(x, y)
        {

        }
    }
}
