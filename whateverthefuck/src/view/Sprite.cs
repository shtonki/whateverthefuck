namespace whateverthefuck.src.view
{
    using System.Drawing;
    using whateverthefuck.src.util;

    public class Sprite
    {
        public Sprite(SpriteID sid)
        {
            this.sid = sid;
        }

        public SpriteID sid { get; protected set; }

        public Image Image() => ImageLoader.GetImage(this.sid);
    }
}
