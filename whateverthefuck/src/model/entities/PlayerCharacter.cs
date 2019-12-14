using System.Drawing;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    public class PlayerCharacter : Character
    {
        private Sprite s;
        public PlayerCharacter(EntityIdentifier identifier, CreationArgs args) : base(identifier, EntityType.PlayerCharacter, args)
        {
            DrawColor = Coloring.RandomColor();
            s = new Sprite(SpriteID.testSprite1);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.DrawSprite(s.sid, Location.X, Location.Y, Size.X, Size.Y);
        }
    }
}
