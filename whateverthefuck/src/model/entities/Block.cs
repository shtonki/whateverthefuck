using System.Drawing;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public class Block : GameEntity
    {
        public Block(EntityIdentifier id, CreationArgs args) : base(id, EntityType.Block, args)
        {
            Height = 100;

            var bca = new BlockCreationArgs(args);
            DrawColor = bca.Color;
        }
    }

    class BlockCreationArgs : CreationArgs
    {
        public enum Types
        {
            Stone,
        }

        public Types Type
        {
            get { return (Types)FirstInt; }
            set { FirstInt = (int)value; }
        }

        public BlockCreationArgs(CreationArgs args) : base(args.Value)
        {
        }

        public BlockCreationArgs(Types type) : base(0)
        {
            Type = type;
        }

        private Color Colorx()
        {
            switch (Type)
            {
                case Types.Stone:
                {
                    return Color.Gray;
                }

            }

            return Color.Black;
        }

        public Color Color => Colorx();
    }
}
