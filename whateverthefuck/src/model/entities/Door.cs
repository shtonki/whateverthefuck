namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    /// <summary>
    /// Doors are like Blocks but you can run through them.
    /// </summary>
    public class Door : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="identifier">The EntityIdentifier of the Door.</param>
        /// <param name="args">The CreationArgs used to create the door.</param>
        public Door(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.Door, args)
        {
            this.Collidable = false;
            this.Height = 15;

            DoorCreationArgs dca = new DoorCreationArgs(args);
            this.DrawColor = dca.Color;
        }
    }

    /// <summary>
    /// The CreationArgs used to create a Door.
    /// </summary>
    public class DoorCreationArgs : CreationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoorCreationArgs"/> class.
        /// </summary>
        /// <param name="args">The CreationArgs to clone.</param>
        public DoorCreationArgs(CreationArgs args)
            : base(args.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoorCreationArgs"/> class.
        /// </summary>
        /// <param name="type">The type of Door to be created.</param>
        public DoorCreationArgs(Types type)
            : base(0)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the Color of the Door to be created.
        /// </summary>
        public Color Color => this.Colorx();

        /// <summary>
        /// The Types of Door which can be created.
        /// </summary>
        public enum Types
        {
            Wood,
            Stone,
        }

        /// <summary>
        /// Gets or sets the Type of the Door to be created.
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
                    return Color.Gray;
                }

                case Types.Wood:
                {
                    return Color.Khaki;
                }
            }

            return Color.Black;
        }
    }
}
