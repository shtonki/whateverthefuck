using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.view
{
    abstract class Drawable
    {
        public Coordinate Location { get; }
        public abstract void Draw(DrawAdapter drawAdapter);
    }
}
