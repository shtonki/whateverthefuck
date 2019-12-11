using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    public class GameCoordinate : Coordinate
    {
        public GameCoordinate(float x, float y) : base(x, y)
        {

        }

        public GameCoordinate Add(float x, float y)
        {
            return new GameCoordinate(X + x, Y + y);
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
        
        public float Distance(GameCoordinate other)
        {
            var xDistance = X - other.X;
            var yDistance = Y - other.Y;
            return (float)Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
        }
    }
}
