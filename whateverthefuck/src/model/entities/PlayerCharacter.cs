namespace whateverthefuck.src.model.entities
{
    using System;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class PlayerCharacter : Character
    {
        private Sprite sprite;

        public PlayerCharacter(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.PlayerCharacter, args)
        {
            this.DrawColor = Coloring.RandomColor();
            this.sprite = new Sprite(SpriteID.testSprite1);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
            drawAdapter.DrawSprite(this.sprite.sid, 0, 0, this.Size.X, this.Size.Y);
        }
    }
}
