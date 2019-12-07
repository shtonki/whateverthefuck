using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.view
{
    public abstract class Drawable
    {
        protected float Rotation { get; set; } = 0;
        public Coordinate Location { get; set; }

        public bool Invisible { get; set; }

        public abstract void DrawMe(DrawAdapter drawAdapter);
        public void Draw(DrawAdapter drawAdapter)
        {
            if (!Invisible)
            {
                DrawMe(drawAdapter);
            }
        }


        protected Drawable(Coordinate location)
        {
            Location = location;
        }
    }
}
