namespace whateverthefuck.src.view
{
    using System.Drawing;
    using whateverthefuck.src.util;

    public class Sprite
    {
        public Sprite(SpriteID sid)
        {
            this.ID = sid;
        }

        public SpriteID ID { get; protected set; }

        public Image Image() => ImageLoader.GetImage(this.ID);
    }
}
