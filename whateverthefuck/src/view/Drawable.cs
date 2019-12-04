using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.view
{
    abstract class Drawable
    {
        protected float Rotation { get; set; } = 0;
        public virtual Coordinate Location { get; set; }
        public abstract void Draw(DrawAdapter drawAdapter);
    }
}
