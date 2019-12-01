using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class GameEntity : Drawable
    {
        int x;
        int y;

        public override void Draw(DrawAdapter drawAdapter)
        {
            float x1 = x / 100f;
            float y1 = y / 100f;
            float x2 = x1 + 0.1f;
            float y2 = y1 + 0.1f;

            drawAdapter.fillRectangle(x1, y1, x2, y2, Color.White);
        }
    }
}
