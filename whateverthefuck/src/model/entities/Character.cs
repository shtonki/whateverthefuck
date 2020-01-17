namespace whateverthefuck.src.model.entities
{
    /// <summary>
    /// Represents a Character in the game.
    /// </summary>
    public abstract class Character : Lootable
    {
        public const float SpeedFast = 0.015f;
        public const float SpeedSlow = 0.01f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        /// <param name="identifier">The EntityIdentifier of the created Character.</param>
        /// <param name="entityType">The EntityType of the created Character.</param>
        /// <param name="args">The CreationArgs used to create the Character.</param>
        public Character(EntityIdentifier identifier, EntityType entityType, CreationArguments args)
            : base(identifier, entityType, args)
        {
            this.Info.Collidable = true;
            this.Info.Movable = true;
            this.Info.Targetable = true;
            this.Info.Height = 1;
        }
    }
}
