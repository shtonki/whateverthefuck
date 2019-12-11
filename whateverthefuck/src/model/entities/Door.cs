using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model.entities
{
    /// <summary>
    /// Doors are defined as holes where there are no blocks.
    /// </summary>
    public class Door : GameEntity
    {
        public Door(EntityIdentifier identifier, CreationArgs args) : base(identifier, EntityType.Door, args)
        {
            Collidable = false;
            Height = 15;

            DoorCreationArgs dca = new DoorCreationArgs(args);
            DrawColor = dca.Color;
        }
    }

    class DoorCreationArgs : CreationArgs
    {
        public enum Types
        {
            Wood,
            Stone,
        }

        public Types Type
        {
            get { return (Types)FirstInt; }
            set { FirstInt = (int)value; }
        }

        public DoorCreationArgs(CreationArgs args) : base(args.Value)
        {
        }

        public DoorCreationArgs(Types type) : base(0)
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

                case Types.Wood:
                {
                    return Color.Khaki;
                }
            }

            return Color.Black;
        }

        public Color Color => Colorx();
    }
}
