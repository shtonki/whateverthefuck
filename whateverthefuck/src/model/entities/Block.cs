namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    /// <summary>
    /// Represents a basic block in the game.
    /// </summary>
    public class Block : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="id">The EntityIdentifier of the created Block.</param>
        /// <param name="args">The CreationArgs used to create the Block.</param>
        public Block(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Block, args)
        {
            this.Height = 100;

            var bca = new BlockCreationArgs(args);
            this.DrawColor = bca.Color;
        }
    }

    /// <summary>
    /// Container for the arguments used for creating a Block.
    /// </summary>
    public class BlockCreationArgs : CreationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCreationArgs"/> class.
        /// </summary>
        /// <param name="args">The CreationArgs to be cloned.</param>
        public BlockCreationArgs(CreationArgs args)
            : base(args.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCreationArgs"/> class.
        /// </summary>
        /// <param name="type">The type of Block to be created.</param>
        public BlockCreationArgs(Types type)
            : base(0)
        {
            this.Type = type;
        }

        /// <summary>
        /// The different types of Block to construct.
        /// </summary>
        public enum Types
        {
            Stone,
        }

        /// <summary>
        /// Gets or sets the type of the Block to be created by the BlockCreationArgs
        /// </summary>
        public Types Type
        {
            get { return (Types)this.FirstInt; }
            set { this.FirstInt = (int)value; }
        }

        /// <summary>
        /// Gets the Color of the Block to be created.
        /// </summary>
        public Color Color => this.Colorx();

        /// <summary>
        /// Looks up the color of a given Type.
        /// </summary>
        /// <returns>The color of the constructed Block.</returns>
        private Color Colorx()
        {
            switch (this.Type)
            {
                case Types.Stone:
                {
                    return Color.Gray;
                }
            }

            return Color.Black;
        }
    }
}
