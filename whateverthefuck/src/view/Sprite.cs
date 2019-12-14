using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.view
{
    public class Sprite
    {
        public SpriteID sid;
        public Image Image() => ImageLoader.GetImage(sid);

        public Sprite(SpriteID sid)
        {
            this.sid = sid;
        }
    }
}
