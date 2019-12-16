namespace whateverthefuck.src.model.entities
{
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class PlayerCharacter : Character
    {
        public PlayerCharacter(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.PlayerCharacter, args)
        {
            this.DrawColor = Coloring.RandomColor();
            this.Sprite = new Sprite(SpriteID.testSprite1);
        }
    }
}
