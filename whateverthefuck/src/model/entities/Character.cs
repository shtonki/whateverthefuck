namespace whateverthefuck.src.model.entities
{
    /// <summary>
    /// Represents a Character in the game.
    /// </summary>
    public abstract class Character : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        /// <param name="identifier">The EntityIdentifier of the created Character.</param>
        /// <param name="entityType">The EntityType of the created Character.</param>
        /// <param name="args">The CreationArgs used to create the Character.</param>
        public Character(EntityIdentifier identifier, EntityType entityType, CreationArgs args)
            : base(identifier, entityType, args)
        {
            this.Movable = true;
            this.Targetable = true;
            this.ShowHealth = true;
            this.Height = 1;
        }
    }
}
