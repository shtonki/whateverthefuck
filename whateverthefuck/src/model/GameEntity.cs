namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.util;
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
        Mankey,

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
    public abstract class GameEntity
    {
        private const float HealthbarWidth = 0.1f;
        private const float HealthbarHeight = 0.02f;

        protected List<Ability> abilities = new List<Ability>();

        private int globalCooldownTicks = 100;
        private int currentGlobalCooldown = 0;

        protected GameEntity(EntityIdentifier identifier, EntityType type, CreationArgs args)
        {
            this.Identifier = identifier;
            this.EntityType = type;

            this.CreationArgs = args;

            this.State = GameEntityState.Alive;

            this.GameLocation = new GameCoordinate(0, 0);

            this.BaseStats = new EntityStats();
            this.CurrentStats = new EntityStats();
        }

        /// <summary>
        /// Event for when the GameEntity dies.
        /// </summary>
        public event Action<GameEntity, GameEntity> OnDeath;

        /// <summary>
        /// Event for when the GameEntity is stepped.
        /// </summary>
        public event Action<GameEntity, GameState> OnStep;

        public int Height { get; protected set; } = 1;

        public GameCoordinate GameLocation { get; set; }

        public EntityStats BaseStats { get; }

        public EntityStats CurrentStats { get; }

        public List<Status> ActiveStatuses { get; } = new List<Status>();

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

        public bool Destroy { get; set; }

        public int LOSGraceTicks { get; set; } = 0;

        /// <summary>
        /// Gets or sets current movement being made by the GameEntity.
        /// </summary>
        public MovementStruct Movements { get; set; } = new MovementStruct();

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
        public float Left => this.GameLocation.X;

        /// <summary>
        /// Gets the X value of the right edge of the GameEntity.
        /// </summary>
        public float Right => this.GameLocation.X + this.Size.X;

        /// <summary>
        /// Gets the Y value of the bottom edge of the GameEntity.
        /// </summary>
        public float Bottom => this.GameLocation.Y;

        /// <summary>
        /// Gets the Y value of the top edge of the GameEntity.
        /// </summary>
        public float Top => this.GameLocation.Y + this.Size.Y;

        /// <summary>
        /// Gets or sets the GameCoordinate in the center of the GameEntity.
        /// </summary>
        public GameCoordinate Center
        {
            get { return new GameCoordinate(this.GameLocation.X + (this.Size.X / 2), this.GameLocation.Y + (this.Size.Y / 2)); }
            set { this.GameLocation = new GameCoordinate(value.X - (this.Size.X / 2), value.Y - (this.Size.Y / 2)); }
        }

        public GameEntityState State { get; protected set; }

        public Color HighlightColor { get; set; } = Color.Transparent;

        protected CastingInfo CastingInfo { get; set; }

        protected Color DrawColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets a value indicating whether or not to show the health bar of the GameEntity.
        /// </summary>
        protected bool ShowHealth { get; set; } = false;

#if false
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

            if (this.Sprite != null)
            {
                drawAdapter.DrawSprite(x1, y1, x2, y2, this.Sprite);
            }
            else
            {
                drawAdapter.FillRectangle(x1, y1, x2, y2, this.DrawColor);
            }

            if (this.HighlightColor != Color.Transparent)
            {
                Rectangle r = new view.Rectangle(x1, y1, this.Size.X, this.Size.Y, this.HighlightColor);
                r.DrawMe(drawAdapter);
            }

            if (this.CastingInfo != null)
            {
                float castingPercentage = this.CastingInfo.PercentageDone;

                var xorg = HealthbarWidth / 2;
                var xoffset = this.Size.X / 2;

                var currentHealthBar = new view.Rectangle(
                    -xorg + xoffset,
                    0,
                    HealthbarWidth * castingPercentage,
                    HealthbarHeight,
                    Color.Gold);

                var maxHealthBar = new view.Rectangle(
                    -xorg + xoffset,
                    0,
                    HealthbarWidth,
                    HealthbarHeight,
                    Color.DarkGray);

                maxHealthBar.DrawMe(drawAdapter);
                currentHealthBar.DrawMe(drawAdapter);
            }

            if (this.ShowHealth)
            {
                float healthPercentage = (float)this.CurrentHealth / this.MaxHealth;

                var xorg = HealthbarWidth / 2;
                var xoffset = this.Size.X / 2;

                var currentHealthBar = new view.Rectangle(
                    -xorg + xoffset,
                    this.Size.Y,
                    HealthbarWidth * healthPercentage,
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

#endif

        /// <summary>
        /// Steps the GameEntity advancing its state by one (1) tick.
        /// </summary>
        /// <param name="gameState">The GameState in which the GameEntity is ticked.</param>
        public virtual void Step(GameState gameState)
        {
            foreach (var status in this.ActiveStatuses)
            {
                status.Duration--;
            }

            this.ActiveStatuses.RemoveAll(s => s.Duration <= 0);

            this.UpdateCurrentStats();

            if (this.CurrentHealth <= 0 && this.State != GameEntityState.Dead)  
            {
                this.Die(gameState);
            }

            if (this.State == GameEntityState.Dead)
            {
                return;
            }

            this.MovementCache = this.CalculateMovement(gameState);
            this.GameLocation += this.MovementCache;

            if (this is PlayerCharacter)
            {
                Boombox.SetListenerPosition(this.GameLocation.X, this.GameLocation.Y);
            }

            if (this.CastingInfo != null)
            {
                if (this.Movements.IsMoving && !this.CanMoveWhileCasting(this.CastingInfo.CastingAbility))
                {
                    // cancel the cast
                    this.CastingInfo = null;
                    this.currentGlobalCooldown = 0;
                }
                else
                {
                    this.CastingInfo.Step();
                    if (this.CastingInfo.DoneCasting)
                    {
                        gameState.HandleGameEvents(new EndCastAbility(
                            this,
                            this.CastingInfo.Target,
                            this.CastingInfo.CastingAbility));

                        this.CastingInfo = null;
                    }
                }
            }

            if (this.currentGlobalCooldown > 0)
            {
                this.currentGlobalCooldown--;
            }

            foreach (var a in this.abilities)
            {
                if (a.CurrentCooldown > 0)
                {
                    a.CurrentCooldown--;
                }
            }

            this.OnStep?.Invoke(this, gameState);
        }

        public void ApplyStatus(Status status)
        {
            var activeStatus = this.ActiveStatuses.FirstOrDefault(s => s.Type == status.Type);

            if (activeStatus != null)
            {
                activeStatus.Stacks += status.Stacks;
                activeStatus.Duration = Math.Max(activeStatus.Duration, status.Duration);
            }
            else
            {
                this.ActiveStatuses.Add(status);
            }
        }

        public Ability Ability(int index)
        {
            return this.abilities[index];
        }

        public Ability Ability(AbilityType abilityType)
        {
            return this.abilities.First(a => a.AbilityType == abilityType);
        }

        public IEnumerable<Ability> CastableAbilities(GameEntity target, GameState gameState)
        {
            return this.abilities.Where(a => this.CanCastAbility(a, target, gameState));
        }

        public bool CanCastAbility(Ability ability, GameEntity target, GameState gameState)
        {
            if (this.currentGlobalCooldown > 0)
            {
                return false;
            }

            if (ability.CurrentCooldown > 0)
            {
                return false;
            }

            if (!ability.TargetingRule.ApplyRule(this, target, gameState))
            {
                return false;
            }

            if (ability.Range < this.DistanceTo(target))
            {
                return false;
            }

            return true;
        }

        public void CastAbility(Ability ability, GameEntity target)
        {
            if (this.CastingInfo != null)
            {
                return;
            }

            if (!this.CanMoveWhileCasting(ability) && this.Movements.IsMoving)
            {
                return;
            }

            this.currentGlobalCooldown = this.globalCooldownTicks;

            ability.CurrentCooldown = ability.BaseCooldown;
            this.CastingInfo = new CastingInfo(ability, target);
        }

        public float CooldownPercentage(Ability ability)
        {
            var globalCooldownPercentage = (float)this.currentGlobalCooldown / this.globalCooldownTicks;
            var abilityCooldownPercentage = ability.CooldownPercentage;

            if (this.currentGlobalCooldown > ability.CurrentCooldown)
            {
                return globalCooldownPercentage;
            }
            else
            {
                return abilityCooldownPercentage;
            }
        }

        public float DistanceTo(GameEntity other)
        {
            return this.DistanceTo(other.Center);
        }

        public float DistanceTo(GameCoordinate location)
        {
            return Coordinate.DistanceBetweenCoordinates(this.Center, location);
        }

        public override string ToString()
        {
            return string.Format("{0} at {1}:{2}", this.EntityType.ToString(), this.GameLocation.X.ToString(), this.GameLocation.Y.ToString());
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
            if (!this.Movable)
            {
                return new GameCoordinate(0, 0);
            }

            if (this.Movements.FollowId.HasValue)
            {
                var followed = gameState.GetEntityById(this.Movements.FollowId.Value);

                if (followed != null)
                {
                    var followingToLocation = followed.Center;

                    if (Coordinate.DistanceBetweenCoordinates(this.Center, followingToLocation) < this.CurrentStats.MoveSpeed)
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
                return new GameCoordinate((float)Math.Sin(this.Movements.Direction) * this.CurrentStats.MoveSpeed, (float)Math.Cos(this.Movements.Direction) * this.CurrentStats.MoveSpeed);
            }
        }

        private void UpdateCurrentStats()
        {
            this.CurrentStats.MoveSpeed = this.BaseStats.MoveSpeed;

            var sanic = this.ActiveStatuses.FirstOrDefault(s => s.Type == Statuses.Sanic);
            if (sanic != null)
            {
                this.CurrentStats.MoveSpeed *= 3;
            }
        }

        private bool CanMoveWhileCasting(Ability ability)
        {
            if (ability.CastTime == 0)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Contains the movement info of a GameEntity.
    /// </summary>
    public class MovementStruct
    {
        private const float NoDirection = float.NaN;

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

        public void Stop()
        {
            this.Direction = NoDirection;
            this.FollowId = null;
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
