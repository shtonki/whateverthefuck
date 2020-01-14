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

            this.Status.BaseStats.MaxHealth = 1;
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

        public EntityStatus Status { get; } = new EntityStatus();

        public GameCoordinate Size { get; set; } = new GameCoordinate(0.1f, 0.1f);

        public EntityType EntityType { get; }

        /// <summary>
        /// Gets the CreationArgs used to create the GameEntity.
        /// </summary>
        public CreationArgs CreationArgs { get; private set; } = null;

        // @move to EntityMovements
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
        public EntityMovement Movements { get; set; } = new EntityMovement();

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

        public bool Visible { get; set; } = true;

        public Sprite Sprite { get; protected set; } = new Sprite(SpriteID.testSprite1);

        public Color HighlightColor { get; set; } = Color.Transparent;

        protected CastingInfo CastingInfo { get; set; }

        protected Color DrawColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets a value indicating whether or not to show the health bar of the GameEntity.
        /// </summary>
        protected bool ShowHealth { get; set; } = false;

        /// <summary>
        /// Steps the GameEntity advancing its state by one (1) tick.
        /// </summary>
        /// <param name="gameState">The GameState in which the GameEntity is ticked.</param>
        public virtual void Step(GameState gameState)
        {
            this.UpdateCurrentStats();

            if (this.Status.ReadCurrentStats.Health <= 0 && this.State != GameEntityState.Dead)
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
            this.OnDeath?.Invoke(this, gameState.GetEntityById(this.LastDamageTaken.AttackerIdentifier));
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

            if (this.Movements.IsFollowing)
            {
                var followed = gameState.GetEntityById(this.Movements.FollowId);

                if (followed != null)
                {
                    var followingToLocation = followed.Center;

                    if (Coordinate.DistanceBetweenCoordinates(this.Center, followingToLocation) < this.Status.ReadCurrentStats.MoveSpeed)
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
                return new GameCoordinate((float)Math.Sin(this.Movements.Direction) * this.Status.ReadCurrentStats.MoveSpeed, (float)Math.Cos(this.Movements.Direction) * this.Status.ReadCurrentStats.MoveSpeed);
            }
        }

        private void UpdateCurrentStats()
        {
            this.Status.Step();
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
