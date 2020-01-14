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
        public Door(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.Door, args)
        {
            this.Collidable = false;
            this.Height = 15;

            DoorCreationArgs dca = new DoorCreationArgs(args);
            this.Sprite = new Sprite(dca.GetSpriteID());
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
            Stone,
        }

        public Types Type
        {
            get { return (Types)this.FirstInt; }
            set { this.FirstInt = (int)value; }
        }

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
    }
}
