namespace whateverthefuck.src.model.entities
{
    using System.Drawing;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    /// <summary>
    /// Represents a basic block in the game.
    /// </summary>
    public class Block : GameEntity
    {
        public Block(EntityIdentifier id, CreationArguments args)
            : this(id, args as BlockCreationArguments)
        {
        }

        public Block(EntityIdentifier id, BlockCreationArguments args)
            : base(id, EntityType.Block, args)
        {
            this.Info.Height = 100;
            this.Info.Collidable = true;

            this.Sprite = new Sprite(args.GetSpriteID());
        }
    }

    public class BlockCreationArguments : CreationArguments
    {
        public BlockCreationArguments()
        {

        }

        public BlockCreationArguments(Types type)
        {
            this.Type = type;
        }

        public enum Types
        {
            Stone,
        }

        public Types Type { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.Type);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Type = (Types)decoder.DecodeInt();
        }

        public SpriteID GetSpriteID()
        {
            switch (this.Type)
            {
                case Types.Stone:
                {
                    return SpriteID.wall_Stone0;
                }

                default: return SpriteID.testSprite1;
            }
        }
    }
}
