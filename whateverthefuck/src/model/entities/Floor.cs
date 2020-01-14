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
        public Floor(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Floor, args)
        {
            this.Collidable = false;
            this.BlocksLOS = false;
            this.Height = 0;

            var fca = new FloorCreationArgs(args);

            this.Sprite = new Sprite(fca.GetSpriteID());
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
            Wood,
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
                case Types.Wood:
                {
                    return SpriteID.floor_Wood0;
                }

                default: return SpriteID.testSprite1;
            }
        }
    }
}
