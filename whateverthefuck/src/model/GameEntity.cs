using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;
using whateverthefuck.src.view;
using static whateverthefuck.src.model.EntityGenerator;
using Rectangle = whateverthefuck.src.view.Rectangle;

namespace whateverthefuck.src.model
{
    abstract public class GameEntity : Drawable
    {
        public int MaxHealth = 100;
        public int CurrentHealth = 100;

        protected bool ShowHealth = false;

        public event Action<GameEntity> OnDeath;

        public GameCoordinate Size { get; set; } = new GameCoordinate(0.1f, 0.1f);

        public EntityType EntityType { get; }
        public CreationArgs CreationArgs { get; private set; } = null;

        public GameCoordinate MovementCache { get; set; } = new GameCoordinate(0, 0);
        public bool Movable { get; protected set; } = false;

        public bool Collidable = true;

        public bool Targetable = false;

        public bool BlocksLOS { get; protected set; } = true;

        public int LOSGraceTicks = 0;

        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed { get; protected set; } = 0.01f;



        public EntityIdentifier Identifier { get; set; }

        public GameEntity(EntityIdentifier identifier, EntityType type, CreationArgs args) : base(new GameCoordinate(0, 0))
        {
            Identifier = identifier;
            EntityType = type;

            CreationArgs = args;

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

        protected Color DrawColor = Color.Black;

        public Color HighlightColor = Color.Transparent;

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            float x1 = Location.X;
            float y1 = Location.Y;
            float x2 = x1 + Size.X;
            float y2 = y1 + Size.Y;

            drawAdapter.FillRectangle(x1, y1, x2, y2, DrawColor);

            if (HighlightColor != Color.Transparent)
            {
                Rectangle r = new view.Rectangle(this, HighlightColor);
                r.DrawMe(drawAdapter);
            }

            if (ShowHealth)
            {
                float mid = (float)CurrentHealth / MaxHealth;
                var full = new view.Rectangle(Left, Top + 0.01f, Left + Size.X * mid, Top, Color.Green);
                var missing = new view.Rectangle(Left + Size.X * mid, Top + 0.01f, Right, Top, Color.Red);

                full.DrawMe(drawAdapter);
                missing.DrawMe(drawAdapter);
            }
        }

        protected virtual void Die(GameState gameState)
        {
            OnDeath?.Invoke(this);
            gameState.HandleGameEvents(new DestroyEntityEvent(this));
        }

        public virtual void Step(GameState gameState)
        {
            MovementCache = CalculateMovement(gameState);
            Location = (GameCoordinate)Location + MovementCache;

            if (CurrentHealth < 0)
            {
                Die(gameState);
            }
        }

        public virtual GameCoordinate CalculateMovement(GameState gameState)
        {
            if (Movements.FollowId.HasValue)
            {
                var followed = gameState.GetEntityById(Movements.FollowId.Value);

                if (followed != null)
                {
                    var followingToLocation = followed.Center;

                    if (Coordinate.DistanceBetweenCoordinates(Center, followingToLocation) < MoveSpeed)
                    {
                        return followingToLocation - Center;
                    }

                    Movements.Direction = Coordinate.AngleBetweenCoordinates(Center, followingToLocation);
                }
            }

            if (float.IsNaN(Movements.Direction))
            {
                return new GameCoordinate(0, 0);
            }
            else
            {
                return new GameCoordinate((float)Math.Sin(Movements.Direction) * MoveSpeed, (float)Math.Cos(Movements.Direction) * MoveSpeed);
            }
        }

        public float DistanceTo(GameCoordinate other)
        {
            return Coordinate.DistanceBetweenCoordinates(Location, other);
        }

        public int GetMemeCode()
        {
            return (int)((Location.X * 1000) * (Location.Y * 1000) * ((int)EntityType));
        }

        public override string ToString()
        {
            return String.Format("{0} at {1}:{2}", EntityType.ToString(), Location.X.ToString(), Location.Y.ToString());
        }
    }

    public class MovementStruct
    {
        public float Direction { get; set; }
        public int? FollowId { get; set; }


        public bool IsDirectional => float.IsNaN(Direction);
        public bool IsFollowing => FollowId.HasValue;

        public bool IsMoving => IsDirectional || IsFollowing;

        public static MovementStruct Following(GameEntity e)
        {
            var rt = new MovementStruct();
            rt.FollowId = e.Identifier.Id;
            return rt;
        }

        public MovementStruct()
        {
            Direction = float.NaN;
            FollowId = null;
        }

        public static MovementStruct Decode(byte[] bs)
        {
            IEnumerable<byte> bytes = bs;
            bool isFollow = BitConverter.ToBoolean(bytes.ToArray(), 0);
            bytes = bytes.Skip(sizeof(bool));

            var ms = new MovementStruct();

            if (isFollow)
            {
                ms.FollowId = BitConverter.ToInt32(bytes.ToArray(), 0);
            }
            else
            {
                ms.Direction = BitConverter.ToSingle(bytes.ToArray(), 0);
            }

            return ms;

        }
        public byte[] Encode()
        {

            if (FollowId.HasValue)
            {
                return BitConverter.GetBytes(true).Concat(BitConverter.GetBytes(FollowId.Value)).ToArray();
            }
            else
            {
                return BitConverter.GetBytes(false).Concat(BitConverter.GetBytes(Direction)).ToArray();
            }
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

        private static int randomCounter = 1;

        public static EntityIdentifier RandomReserved()
        {
            var v = -randomCounter++;
            if (randomCounter > 100000)
            {
                randomCounter = 1;
            }

            return new EntityIdentifier(v);
        }
    }


    public enum EntityType
    {
        None,

        GameMechanic,

        Projectile,

        PlayerCharacter,
        Block,
        NPC,
        Door,
        Floor,

        Loot,
    }
}
