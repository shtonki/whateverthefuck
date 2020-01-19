namespace whateverthefuck.src.model
{
    public class EntityInfo
    {
        public EntityInfo(EntityType entityType, CreationArguments creationArgs, EntityIdentifier identifier)
        {
            EntityType = entityType;
            CreationArgs = creationArgs;
            Identifier = identifier;
        }

        public EntityIdentifier Identifier { get; }

        public EntityType EntityType { get; }

        public CreationArguments CreationArgs { get; }

        public int Level { get; set; }

        public int Height { get; set; } = 1;

        public GameCoordinate GameLocation { get; set; }

        public GameCoordinate Size { get; set; } = new GameCoordinate(0.1f, 0.1f);

        public bool Movable { get; set; }

        public bool Collidable { get; set; }

        public bool Targetable { get; set; }

        public bool Destroy { get; set; }

        public EntityIdentifier Target { get; set; }

        /// <summary>
        /// Gets the X value of the left edge of the GameEntity.
        /// </summary>
        public float Left => this.GameLocation.X;

        /// <summary>
        /// Gets the X value of the right edge of the GameEntity.
        /// </summary>
        public float Right => this.GameLocation.X + this.Size.X;

        /// <summary>
        /// Gets the Y value of the bottom edge of the GameEntity.
        /// </summary>
        public float Bottom => this.GameLocation.Y;

        /// <summary>
        /// Gets the Y value of the top edge of the GameEntity.
        /// </summary>
        public float Top => this.GameLocation.Y + this.Size.Y;

        /// <summary>
        /// Gets or sets the GameCoordinate in the center of the GameEntity.
        /// </summary>
        public GameCoordinate Center
        {
            get { return new GameCoordinate(this.GameLocation.X + (this.Size.X / 2), this.GameLocation.Y + (this.Size.Y / 2)); }
            set { this.GameLocation = new GameCoordinate(value.X - (this.Size.X / 2), value.Y - (this.Size.Y / 2)); }
        }

        public GameEntityState State { get; set; }

        public bool Visible { get; set; } = true;
    }
}
