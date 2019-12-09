using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using whateverthefuck.src.util;
using whateverthefuck.src.view;
using static whateverthefuck.src.model.EntityGenerator;

namespace whateverthefuck.src.model
{
    abstract public class GameEntity : Drawable
    {

        public GameCoordinate Size { get; set; } = new GameCoordinate(0.1f, 0.1f);
        public int Height { get; protected set; } = 1;

        public EntityType EntityType { get; }

        public GameCoordinate MovementCache { get; set; } = new GameCoordinate(0, 0);
        public bool Movable { get; protected set; } = false;

        public bool Collidable = true;

        public bool BlocksLOS { get; protected set; } = true;

        public int LOSGraceTicks = 0; 

        public EntityIdentifier Identifier { get; set; }

        public GameEntity(EntityIdentifier identifier, EntityType type) : base(new GameCoordinate(0, 0))
        {
            Identifier = identifier;
            EntityType = type;

            //debug
            Visible = true;
        }

        public float Left => Location.X;
        public float Right => Location.X + Size.X;
        public float Bottom => Location.Y;
        public float Top => Location.Y + Size.Y;
        public GameCoordinate Center 
        { 
            get { return new GameCoordinate(Location.X + Size.X / 2, Location.Y + Size.Y / 2); } 
            set { Location = new GameCoordinate(value.X - Size.X / 2, value.Y - Size.Y / 2); }
        }

        private GameCoordinate _location { get; set; }

        protected Color DrawColor = Color.Black;

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            float x1 = Location.X;
            float y1 = Location.Y;
            float x2 = x1 + Size.X;
            float y2 = y1 + Size.Y;

            drawAdapter.FillRectangle(x1, y1, x2, y2, DrawColor);
        }

        public virtual void Step()
        {
            MovementCache = CalculateMovement();
            Location = (GameCoordinate)Location + MovementCache;
        }

        public virtual GameCoordinate CalculateMovement()
        {
            return new GameCoordinate(0, 0);
        }
    }

    public class EntityIdentifier
    {
        public int Id { get; }

        private const int InvalidId = -1;

        public bool IsValid => Id != InvalidId;
        public static EntityIdentifier Invalid => new EntityIdentifier(InvalidId);

        public EntityIdentifier(int id)
        {
            Id = id;
        }
    }


    public enum EntityType
    {
        None,

        GameMechanic,

        PlayerCharacter,
        Block,
        NPC,
        Door,
        Floor,
    }
}
