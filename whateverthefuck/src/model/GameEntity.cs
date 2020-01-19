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

        Test,

        Projectile,

        PC,
        Block,
        NPC,
        Door,
        Floor,
    }

    public enum GameEntityState
    {
        Created,
        Alive,
        Dead,
    }

    /// <summary>
    /// Represents an entity in the game.
    /// </summary>
    public abstract class GameEntity
    {
        protected GameEntity(EntityIdentifier identifier, EntityType type, CreationArguments args)
        {
            this.Abilities = new EntityAbilities(this);
            this.Info = new EntityInfo(type, args, identifier);

            this.Info.State = GameEntityState.Created;

            this.Info.GameLocation = new GameCoordinate(0, 0);
        }

        public event Action<GameEntity, GameState> OnDeath;

        public event Action<GameEntity, GameState> OnStep;

        public event Action<DealDamageEvent, GameState> OnDamaged;

        public event Action<GameEntity> OnInteract;

        public EntityStatus Status { get; protected set; }

        public Equipment Equipment { get; protected set; }

        public EntityAbilities Abilities { get; protected set; }

        public EntityInfo Info { get; }

        public EntityMovement Movements { get; set; } = new EntityMovement();

        // @fix every property below probably belongs somewhere else

        public int LOSGraceTicks { get; set; } = 0;

        /// <summary>
        /// Gets or sets current movement being made by the GameEntity.
        /// </summary>

        // @move to EntityMovements
        public GameCoordinate MovementCache { get; set; } = new GameCoordinate(0, 0);

        public Sprite Sprite { get; protected set; } = new Sprite(SpriteID.testSprite1);

        public Color HighlightColor { get; set; } = Color.Transparent;

        protected Color DrawColor { get; set; } = Color.Black;

        private int CorpseCounter { get; set; }

        /// <summary>
        /// Steps the GameEntity advancing its state by one (1) tick.
        /// </summary>
        /// <param name="gameState">The GameState in which the GameEntity is ticked.</param>
        public virtual void Step(GameState gameState)
        {
            this.UpdateCurrentStats(gameState);

            if (this.Status != null)
            {
                // handle status

                if (this.Status.ReadCurrentStats.Health <= 0 && this.Info.State != GameEntityState.Dead)
                {
                    this.Die(gameState);
                }

                if (this.Info.State == GameEntityState.Dead)
                {
                    if (this.CorpseCounter++ > 500)
                    {
                        this.Info.Destroy = true;
                    }

                    return;
                }
            }

            this.MovementCache = this.CalculateMovement(gameState);
            this.Info.GameLocation += this.MovementCache;

            // @pls
            if (this is PC)
            {
                Boombox.SetListenerPosition(this.Info.GameLocation.X, this.Info.GameLocation.Y);
            }

            this.Abilities?.Step(gameState);

            this.OnStep?.Invoke(this, gameState);
        }

        public void Reset()
        {
            // @incomplete reset everything
            this.Status?.ResetToBaseStats();
            this.Info.State = GameEntityState.Alive;
        }

        public void ReceiveDamage(DealDamageEvent e, GameState gs)
        {
            this.Status.WriteCurrentStats.Health -= e.Damage;

            if (this.Status.ReadCurrentStats.Health < 0)
            {
                this.Status.WriteCurrentStats.Health = 0;
            }

            this.OnDamaged?.Invoke(e, gs);
        }

        public void ReceiveHealing(HealEvent e, GameState gs)
        {
            var headRoom = this.Status.ReadCurrentStats.MaxHealth - this.Status.ReadCurrentStats.Health;
            var healing = Math.Min(e.Healing, headRoom);

            this.Status.WriteCurrentStats.Health += healing;
        }

        public void Interact()
        {
            OnInteract?.Invoke(this);
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
            this.OnDeath?.Invoke(this, gameState);
            this.Info.State = GameEntityState.Dead;
            this.Abilities.StopCasting();
            this.Status.ActiveStatuses.Clear();
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
            this.Status?.Step(this, gameState);
            this.Equipment?.ApplyStaticEffects(this.Status.ReadCurrentStats);
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
