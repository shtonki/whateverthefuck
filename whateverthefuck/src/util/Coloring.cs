using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.util
{
    static class Coloring
    {
        //https://xkcd.com/221/
        static Random r = new Random(3434);

        public static Color RandomColor()
        {
            return Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255));
        }

        public static Color Opposite(Color c)
        {
            return Color.FromArgb(c.ToArgb() ^ 0xffffff);
        }
    }
}
