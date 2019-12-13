namespace whateverthefuck.src.model.entities
{
    using whateverthefuck.src.util;

    public class PlayerCharacter : Character
    {
        public PlayerCharacter(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.PlayerCharacter, args)
        {
            this.DrawColor = Coloring.RandomColor();
        }
    }
}
