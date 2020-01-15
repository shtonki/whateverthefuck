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

        Projectile,

        PC,
        Block,
        NPC,
        Door,
        Floor,
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

        private int globalCooldownTicks = 100;
        private int currentGlobalCooldown = 0;

        protected GameEntity(EntityIdentifier identifier, EntityType type, CreationArguments args)
        {
            this.Abilities = new EntityAbilities(this);
            this.Info = new EntityInfo(type, args, identifier);

            this.Info.State = GameEntityState.Alive;

            this.Info.GameLocation = new GameCoordinate(0, 0);

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

        public EntityStatus Status { get; } = new EntityStatus();

        public EntityAbilities Abilities { get; }

        public EntityInfo Info { get; }

        public EntityMovement Movements { get; set; } = new EntityMovement();

        // @fix every property below probably belongs somewhere else

        public int LOSGraceTicks { get; set; } = 0;

        /// <summary>
        /// Gets or sets current movement being made by the GameEntity.
        /// </summary>

        // @move to EntityMovements
        public GameCoordinate MovementCache { get; set; } = new GameCoordinate(0, 0);

        /// <summary>
        /// Gets or sets the last DealDamageEvent to deal damage to the GameEntity.
        /// </summary>
        public DealDamageEvent LastDamageTaken { get; set; }

        public Sprite Sprite { get; protected set; } = new Sprite(SpriteID.testSprite1);

        public Color HighlightColor { get; set; } = Color.Transparent;

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
            this.UpdateCurrentStats(gameState);

            if (this.Status.ReadCurrentStats.Health <= 0 && this.Info.State != GameEntityState.Dead)
            {
                this.Die(gameState);
            }

            if (this.Info.State == GameEntityState.Dead)
            {
                return;
            }

            this.MovementCache = this.CalculateMovement(gameState);
            this.Info.GameLocation += this.MovementCache;

            if (this is PC)
            {
                Boombox.SetListenerPosition(this.Info.GameLocation.X, this.Info.GameLocation.Y);
            }

            this.Abilities.Step(gameState);

            this.OnStep?.Invoke(this, gameState);
        }

        public float DistanceTo(GameEntity other)
        {
            return this.DistanceTo(other.Info.Center);
        }

        public float DistanceTo(GameCoordinate location)
        {
            return Coordinate.DistanceBetweenCoordinates(this.Info.Center, location);
        }

        public override string ToString()
        {
            return string.Format("{0} at {1}:{2}", this.Info.EntityType.ToString(), this.Info.GameLocation.X.ToString(), this.Info.GameLocation.Y.ToString());
        }

        /// <summary>
        /// This kills the GameEntity.
        /// </summary>
        /// <param name="gameState">Used to look up the GameEntity that killed this GameEntity.</param>
        protected virtual void Die(GameState gameState)
        {
            this.OnDeath?.Invoke(this, gameState.GetEntityById(this.LastDamageTaken.AttackerIdentifier));
            this.Info.State = GameEntityState.Dead;

            this.ShowHealth = false;
        }

        /// <summary>
        /// Calculates the next movement to be made by the GameEntity.
        /// </summary>
        /// <param name="gameState">The GameState in which to calculate movement.</param>
        /// <returns>The Coordinate of the next movement to be made.</returns>
        protected virtual GameCoordinate CalculateMovement(GameState gameState)
        {
            if (!this.Info.Movable)
            {
                return new GameCoordinate(0, 0);
            }

            if (this.Movements.IsFollowing)
            {
                var followed = gameState.GetEntityById(this.Movements.FollowId);

                if (followed != null)
                {
                    var followingToLocation = followed.Info.Center;

                    if (Coordinate.DistanceBetweenCoordinates(this.Info.Center, followingToLocation) < this.Status.ReadCurrentStats.MoveSpeed)
                    {
                        return followingToLocation - this.Info.Center;
                    }

                    this.Movements.Direction = Coordinate.AngleBetweenCoordinates(this.Info.Center, followingToLocation);
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

        private void UpdateCurrentStats(GameState gameState)
        {
            this.Status.Step(this, gameState);
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
