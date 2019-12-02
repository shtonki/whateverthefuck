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
    }
}
