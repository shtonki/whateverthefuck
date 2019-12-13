namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    /// <summary>
    /// Doors are like Blocks but you can run through them.
    /// </summary>
    public class Door : GameEntity
    {
        public Door(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.Door, args)
        {
            this.Collidable = false;
            this.Height = 15;

            DoorCreationArgs dca = new DoorCreationArgs(args);
            this.DrawColor = dca.Color;
        }
    }

    public class DoorCreationArgs : CreationArgs
    {
        public DoorCreationArgs(CreationArgs args)
            : base(args.Value)
        {
        }

        public DoorCreationArgs(Types type)
            : base(0)
        {
            this.Type = type;
        }

        public enum Types
        {
            Wood,
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
