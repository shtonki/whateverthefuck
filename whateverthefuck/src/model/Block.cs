using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    class Block : GameEntity
    {
        public Block(Color color)
        {
            DrawColor = color;
        }

        public Block()
        {
            DrawColor = Coloring.RandomColor();
        }
    }
}
