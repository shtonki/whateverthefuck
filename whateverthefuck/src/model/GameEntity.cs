namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.view;
    using Rectangle = whateverthefuck.src.view.Rectangle;

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

    public enum GameEntityState
    {
        Alive,
        Dead,
    }

    /// <summary>
    /// Represents an entity in the game.
    /// </summary>
    public abstract class GameEntity : Drawable
    {
        private const float HealthbarWidth = 0.1f;
        private const float HealthbarHeight = 0.02f;

        protected GameEntity(EntityIdentifier identifier, EntityType type, CreationArgs args)
            : base(new GameCoordinate(0, 0))
        {
            this.Identifier = identifier;
            this.EntityType = type;

            this.CreationArgs = args;

            this.State = GameEntityState.Alive;

            this.Visible = true;
        }

        /// <summary>
        /// Event for when the GameEntity dies.
        /// </summary>
        public event Action<GameEntity, GameEntity> OnDeath;

        /// <summary>
        /// Event for when the GameEntity is stepped.
        /// </summary>
        public event Action<GameEntity> OnStep;

        public int MaxHealth { get; set; } = 100;

        public int CurrentHealth { get; set; } = 100;

        public GameCoordinate Size { get; set; } = new GameCoordinate(0.1f, 0.1f);

        public EntityType EntityType { get; }

        /// <summary>
        /// Gets the CreationArgs used to create the GameEntity.
        /// </summary>
        public CreationArgs CreationArgs { get; private set; } = null;

        /// <summary>
        /// Gets or sets the last movement the GameEntity made.
        /// </summary>
        public GameCoordinate MovementCache { get; set; } = new GameCoordinate(0, 0);

        /// <summary>
        /// Gets or sets a value indicating whether the GameEntity can move.
        /// </summary>
        public bool Movable { get; protected set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the GameEntity can be collided with.
        /// </summary>
        public bool Collidable { get; protected set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the GameEntity can be targeted.
        /// </summary>
        public bool Targetable { get; protected set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the GameEntity blocks line of sight.
        /// </summary>
        public bool BlocksLOS { get; protected set; } = true;

        public int LOSGraceTicks { get; set; } = 0;

        /// <summary>
        /// Gets or sets current movement being made by the GameEntity.
        /// </summary>
        public MovementStruct Movements { get; set; } = new MovementStruct();

        public float MoveSpeed { get; protected set; } = 0.01f;

        /// <summary>
        /// Gets or sets the last DealDamageEvent to deal damage to the GameEntity.
        /// </summary>
        public DealDamageEvent LastDamageTaken { get; set; }

        /// <summary>
        /// Gets or sets the EntityIdentifier which uniquely identifies a GameEntity in a GameState.
        /// </summary>
        public EntityIdentifier Identifier { get; protected set; }

        /// <summary>
        /// Gets the X value of the left edge of the GameEntity.
        /// </summary>
        public float Left => this.Location.X;

        /// <summary>
        /// Gets the X value of the right edge of the GameEntity.
        /// </summary>
        public float Right => this.Location.X + this.Size.X;

        /// <summary>
        /// Gets the Y value of the bottom edge of the GameEntity.
        /// </summary>
        public float Bottom => this.Location.Y;

        /// <summary>
        /// Gets the Y value of the top edge of the GameEntity.
        /// </summary>
        public float Top => this.Location.Y + this.Size.Y;

        /// <summary>
        /// Gets or sets the GameCoordinate in the center of the GameEntity.
        /// </summary>
        public GameCoordinate Center
        {
            get { return new GameCoordinate(this.Location.X + (this.Size.X / 2), this.Location.Y + (this.Size.Y / 2)); }
            set { this.Location = new GameCoordinate(value.X - (this.Size.X / 2), value.Y - (this.Size.Y / 2)); }
        }

        public GameEntityState State { get; protected set; }

        public Color HighlightColor { get; set; } = Color.Transparent;

        protected Color DrawColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets a value indicating whether or not to show the health bar of the GameEntity.
        /// </summary>
        protected bool ShowHealth { get; set; } = false;


        /// <summary>
        /// The function used to draw the GameEntity.
        /// </summary>
        /// <param name="drawAdapter">The DrawAdapter used to draw the GameEntity.</param>
        public override void DrawMe(DrawAdapter drawAdapter)
        {
            float x1 = 0;
            float y1 = 0;
            float x2 = x1 + this.Size.X;
            float y2 = y1 + this.Size.Y;

            drawAdapter.FillRectangle(x1, y1, x2, y2, this.DrawColor);

            if (this.HighlightColor != Color.Transparent)
            {
                Rectangle r = new view.Rectangle(x1, y1, Size.X, Size.Y, this.HighlightColor);
                r.DrawMe(drawAdapter);
            }

            if (this.ShowHealth)
            {
                float healthPercentageDividedBy100ForClarity = (float)this.CurrentHealth / this.MaxHealth;

                var xorg = HealthbarWidth / 2;
                var xoffset = this.Size.X / 2;

                var currentHealthBar = new view.Rectangle(
                    -xorg + xoffset,
                    this.Size.Y,
                    HealthbarWidth * healthPercentageDividedBy100ForClarity,
                    HealthbarHeight,
                    Color.Green);

                var maxHealthBar = new view.Rectangle(
                    -xorg + xoffset,
                    this.Size.Y,
                    HealthbarWidth,
                    HealthbarHeight,
                    Color.Red);

                maxHealthBar.DrawMe(drawAdapter);
                currentHealthBar.DrawMe(drawAdapter);
            }
        }

        /// <summary>
        /// Steps the GameEntity advancing its state by one (1) tick.
        /// </summary>
        /// <param name="gameState">The GameState in which the GameEntity is ticked.</param>
        public virtual void Step(GameState gameState)
        {
            if (this.State == GameEntityState.Dead)
            {
                return;
            }

            this.OnStep?.Invoke(this);

            this.MovementCache = this.CalculateMovement(gameState);
            this.Location = (GameCoordinate)this.Location + this.MovementCache;

            if (this.CurrentHealth < 0)
            {
                this.Die(gameState);
            }
        }

        /// <summary>
        /// Calculates the distance to another GameEntity.
        /// </summary>
        /// <param name="other">The GameEntity to which distance is to be calculated.</param>
        /// <returns>The distance to the other GameEntity.</returns>
        public float DistanceTo(GameCoordinate other)
        {
            return Coordinate.DistanceBetweenCoordinates(this.Location, other);
        }

        public override string ToString()
        {
            return string.Format("{0} at {1}:{2}", this.EntityType.ToString(), this.Location.X.ToString(), this.Location.Y.ToString());
        }

        /// <summary>
        /// This kills the GameEntity.
        /// </summary>
        /// <param name="gameState">Used to look up the GameEntity that killed this GameEntity.</param>
        protected virtual void Die(GameState gameState)
        {
            this.OnDeath?.Invoke(this, gameState.GetEntityById(this.LastDamageTaken.AttackerId));
            this.State = GameEntityState.Dead;

            this.ShowHealth = false;
        }

        /// <summary>
        /// Calculates the next movement to be made by the GameEntity.
        /// </summary>
        /// <param name="gameState">The GameState in which to calculate movement.</param>
        /// <returns>The Coordinate of the next movement to be made.</returns>
        protected virtual GameCoordinate CalculateMovement(GameState gameState)
        {
            if (this.Movements.FollowId.HasValue)
            {
                var followed = gameState.GetEntityById(this.Movements.FollowId.Value);

                if (followed != null)
                {
                    var followingToLocation = followed.Center;

                    if (Coordinate.DistanceBetweenCoordinates(this.Center, followingToLocation) < this.MoveSpeed)
                    {
                        return followingToLocation - this.Center;
                    }

                    this.Movements.Direction = Coordinate.AngleBetweenCoordinates(this.Center, followingToLocation);
                }
            }

            if (float.IsNaN(this.Movements.Direction))
            {
                return new GameCoordinate(0, 0);
            }
            else
            {
                return new GameCoordinate((float)Math.Sin(this.Movements.Direction) * this.MoveSpeed, (float)Math.Cos(this.Movements.Direction) * this.MoveSpeed);
            }
        }
    }

    /// <summary>
    /// Contains the movement info of a GameEntity.
    /// </summary>
    public class MovementStruct
    {
        public MovementStruct()
        {
            this.Direction = float.NaN;
            this.FollowId = null;
        }

        /// <summary>
        /// Gets or sets the direction of movement in radians, or NaN if not moving directionally.
        /// </summary>
        public float Direction { get; set; }

        /// <summary>
        /// Gets or sets the id of the followed entity, or null if not following anything.
        /// </summary>
        public int? FollowId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the GameEntity is moving in a direction.
        /// </summary>
        public bool IsDirectional => !float.IsNaN(this.Direction);

        /// <summary>
        /// Gets a value indicating whether the GameEntity is following another GameEntity.
        /// </summary>
        public bool IsFollowing => this.FollowId.HasValue;

        /// <summary>
        /// Gets a value indicating whether the GameEntity is moving at all.
        /// </summary>
        public bool IsMoving => this.IsDirectional || this.IsFollowing;

        /// <summary>
        /// Decodes an array of bytes into a MovementStruct.
        /// </summary>
        /// <param name="bs">The bytes to decode.</param>
        /// <returns>The decoded MovementStruct.</returns>
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

        /// <summary>
        /// Encodes a MovementStruct into a byte[].
        /// </summary>
        /// <returns>The bytes representing the MovementStruct.</returns>
        public byte[] Encode()
        {
            if (this.FollowId.HasValue)
            {
                return BitConverter.GetBytes(true).Concat(BitConverter.GetBytes(this.FollowId.Value)).ToArray();
            }
            else
            {
                return BitConverter.GetBytes(false).Concat(BitConverter.GetBytes(this.Direction)).ToArray();
            }
        }
    }

    /// <summary>
    /// Uniquely identifies a GameEntity within a GameState.
    /// </summary>
    public class EntityIdentifier
    {
        private const int InvalidId = -1;

        private static int randomCounter = 1;

        public EntityIdentifier(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets an new invalid EntityIdentifier.
        /// </summary>
        public static EntityIdentifier Invalid => new EntityIdentifier(InvalidId);

        /// <summary>
        /// Gets the Identifier of the GameEntity.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets a value indicating whether the EntityIdentifier is valid.
        /// </summary>
        public bool IsValid => this.Id != InvalidId;

        /// <summary>
        /// Creates the next reserved EntityIdentifier.
        /// Will cause all hell to break loose if shipped over the network.
        /// </summary>
        /// <returns>The next reserved EntityIdentifier.</returns>
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
}
