namespace whateverthefuck.src.model.entities
{
    using System.Drawing;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    /// <summary>
    /// Doors are like Blocks but you can run through them.
    /// </summary>
    public class Door : GameEntity
    {
        public Door(EntityIdentifier id, CreationArguments args)
            : this(id, args as DoorCreationArguments)
        {
        }

        public Door(EntityIdentifier identifier, DoorCreationArguments args)
            : base(identifier, EntityType.Door, args)
        {
            this.Collidable = false;
            this.Height = 15;

            this.Sprite = new Sprite(args.GetSpriteID());
        }
    }

    public class DoorCreationArguments : CreationArguments
    {


        public DoorCreationArguments(Types type)
        {
            this.Type = type;
        }

        public DoorCreationArguments()
        {
        }

        public enum Types
        {
            Stone,
        }

        public Types Type { get; private set; }

        public SpriteID GetSpriteID()
        {
            switch (this.Type)
            {
                case Types.Stone:
                {
                    return SpriteID.door_Stone0;
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
