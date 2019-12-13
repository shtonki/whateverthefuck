namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    /// <summary>
    /// Represents a basic block in the game.
    /// </summary>
    public class Block : GameEntity
    {
        public Block(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Block, args)
        {
            this.Height = 100;

            var bca = new BlockCreationArgs(args);
            this.DrawColor = bca.Color;
        }
    }

    public class BlockCreationArgs : CreationArgs
    {
        public BlockCreationArgs(CreationArgs args)
            : base(args.Value)
        {
        }

        public BlockCreationArgs(Types type)
            : base(0)
        {
            this.Type = type;
        }

        public enum Types
        {
            Stone,
        }

        public Types Type
        {
            get { return (Types)this.FirstInt; }
            set { this.FirstInt = (int)value; }
        }

        public Color Color => this.Colorx();

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
