using System.Drawing;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class Hero : Character
    {
        public Hero()
        {
            DrawColor = Coloring.RandomColor();
        }
    }
}
