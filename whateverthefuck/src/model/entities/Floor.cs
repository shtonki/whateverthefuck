namespace whateverthefuck.src.model.entities
{
    using System.Drawing;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    /// <summary>
    /// Represents a floor tile.
    /// </summary>
    public class Floor : GameEntity
    {
        public Floor(EntityIdentifier id, CreationArguments args)
            : this(id, args as FloorCreationArguments)
        {
        }

        public Floor(EntityIdentifier id, FloorCreationArguments args)
            : base(id, EntityType.Floor, args)
        {
            this.Collidable = false;
            this.BlocksLOS = false;
            this.Height = 0;

            this.Sprite = new Sprite(args.GetSpriteID());
        }
    }

    public class FloorCreationArguments : CreationArguments
    {
        public FloorCreationArguments(Types type)
        {
            this.Type = type;
        }

        public FloorCreationArguments()
        {
        }

        public enum Types
        {
            Wood,
        }

        public Types Type { get; private set; }

        public SpriteID GetSpriteID()
        {
            switch (this.Type)
            {
                case Types.Wood:
                {
                    return SpriteID.floor_Wood0;
                }

                default: return SpriteID.testSprite1;
            }
        }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.Type);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Type = (Types)decoder.DecodeInt();
        }
    }
}
