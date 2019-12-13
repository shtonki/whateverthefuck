namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    /// <summary>
    /// Represents a floor tile.
    /// </summary>
    public class Floor : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Floor"/> class.
        /// </summary>
        /// <param name="id">The EntityIdentifier of the Floor.</param>
        /// <param name="args">The CreationArgs used to create the Floor.</param>
        public Floor(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Floor, args)
        {
            this.Collidable = false;
            this.BlocksLOS = false;
            this.Height = 0;

            var fca = new FloorCreationArgs(args);
            this.DrawColor = fca.Color;
        }
    }

    /// <summary>
    /// The CreationArgs used to create a Floor.
    /// </summary>
    public class FloorCreationArgs : CreationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorCreationArgs"/> class.
        /// </summary>
        /// <param name="args">The CreationArgs to be cloned.</param>
        public FloorCreationArgs(CreationArgs args)
            : base(args.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorCreationArgs"/> class.
        /// </summary>
        /// <param name="type">The Type of Floor to be created.</param>
        public FloorCreationArgs(Types type)
            : base(0)
        {
            this.Type = type;
        }

        /// <summary>
        /// The different types of Floor.
        /// </summary>
        public enum Types
        {
            Grass,
            Stone,
        }

        /// <summary>
        /// Gets the Color of the Floor to be created.
        /// </summary>
        public Color Color => this.Colorx();

        /// <summary>
        /// Gets or sets the Type of Floor to be created.
        /// </summary>
        public Types Type
        {
            get { return (Types)this.FirstInt; }
            set { this.FirstInt = (int)value; }
        }

        private Color Colorx()
        {
            switch (this.Type)
            {
                case Types.Stone:
                    {
                        return Color.Silver;
                    }

                case Types.Grass:
                    {
                        return Color.Green;
                    }
            }

            return Color.Black;
        }
    }
}
