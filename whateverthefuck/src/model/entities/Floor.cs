namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    /// <summary>
    /// Represents a floor tile.
    /// </summary>
    public class Floor : GameEntity
    {
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

    public class FloorCreationArgs : CreationArgs
    {
        public FloorCreationArgs(CreationArgs args)
            : base(args.Value)
        {
        }

        public FloorCreationArgs(Types type)
            : base(0)
        {
            this.Type = type;
        }

        public enum Types
        {
            Grass,
            Stone,
        }

        public Color Color => this.Colorx();

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
