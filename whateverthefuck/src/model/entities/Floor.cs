using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    public class Floor : GameEntity
    {
        public Floor(EntityIdentifier id, CreationArgs args) : base(id, EntityType.Floor, args)
        {
            Collidable = false;
            BlocksLOS = false;
            Height = 0;

            var fca = new FloorCreationArgs(args);
            DrawColor = fca.Color;
        }
    }

    class FloorCreationArgs : CreationArgs
    {
        public enum Types
        {
            Grass,
            Stone,
        }

        public Types Type
        {
            get { return (Types)FirstInt; }
            set { FirstInt = (int)value; }
        }

        public FloorCreationArgs(CreationArgs args) : base(args.Value)
        {
        }

        public FloorCreationArgs(Types type) : base(0)
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

                case Types.Grass:
                    {
                        return Color.Green;
                    }
            }

            return Color.Black;
        }

        public Color Color => Colorx();
    }
}
