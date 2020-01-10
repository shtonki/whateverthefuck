namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using whateverthefuck.src.util;

    /// <summary>
    /// Represents an event which alters a GameState.
    /// </summary>
    public abstract class GameEvent
    {
        protected GameEvent(GameEventType type)
        {
            this.Type = type;
        }

        public GameEventType Type { get; protected set; }
    }

    public class ApplyStatusEvent : GameEvent
    {
        public ApplyStatusEvent(GameEntity entity, Status appliedStatus)
            : base(GameEventType.ApplyStatus)
        {
            this.EntityIdentifier = entity.Identifier;
            this.Status = appliedStatus;
        }

        public EntityIdentifier EntityIdentifier { get; }

        public Status Status { get; }
    }

    public class BeginCastAbilityEvent : GameEvent
    {
        public BeginCastAbilityEvent(GameEntity caster, GameEntity target, Ability ability)
            : base(GameEventType.UseAbility)
        {
            this.CasterIdentifier = caster.Identifier;
            this.AbilityType = ability.AbilityType;
            this.TargetIdentifier = target.Identifier;
        }

        // @consider changing this to the index of the ability within the casters ability array
        public AbilityType AbilityType { get; private set; }

        public EntityIdentifier CasterIdentifier { get; private set; }

        public EntityIdentifier TargetIdentifier { get; private set; }
    }

    public class EndCastAbility : GameEvent
    {
        public EndCastAbility(GameEntity caster, GameEntity target, Ability ability)
            : base(GameEventType.UseAbility)
        {
            this.CasterIdentifier = caster.Identifier;
            this.AbilityType = ability.AbilityType;
            this.TargetIdentifier = target.Identifier;
        }

        public AbilityType AbilityType { get; private set; }

        public EntityIdentifier CasterIdentifier { get; private set; }

        public EntityIdentifier TargetIdentifier { get; private set; }
    }

    public class CreateEntityEvent : GameEvent
    {
        public CreateEntityEvent(GameEntity e)
            : base(GameEventType.Create)
        {
            this.Id = e.Identifier.Id;
            this.EntityType = e.EntityType;
            this.X = e.GameLocation.X;
            this.Y = e.GameLocation.Y;
            this.CurrentHealth = e.CurrentHealth;
            this.MaxHealth = e.MaxHealth;

            if (e.CreationArgs == null)
            {
                this.CreationArgs = new CreationArgs(0);
            }
            else
            {
                this.CreationArgs = e.CreationArgs;
            }
        }

        public CreateEntityEvent(EntityIdentifier id, EntityType entityType, float x, float y, int currentHealth, int maxHealth, CreationArgs creationArgs)
            : base(GameEventType.Create)
        {
            this.Id = id.Id;
            this.EntityType = entityType;
            this.X = x;
            this.Y = y;
            this.CurrentHealth = currentHealth;
            this.MaxHealth = maxHealth;
            this.CreationArgs = creationArgs;
        }

        public int Id { get; private set; }

        public EntityType EntityType { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public int CurrentHealth { get; private set; }

        public int MaxHealth { get; private set; }

        public CreationArgs CreationArgs { get; private set; }

        public Action<GameEntity> OnCreationCallback { get; set; }

        public Action<GameEntity, GameEntity> OnDeathCallback { get; set; }

        public Action<GameEntity, GameState> OnStepCallback { get; set; }
    }

    public class DealDamageEvent : GameEvent
    {
        public DealDamageEvent(GameEntity attacker, GameEntity defender, int damage)
            : base(GameEventType.Damage)
        {
            this.Type = GameEventType.Damage;

            this.AttackerIdentifier = attacker.Identifier;
            this.DefenderIdentifier = defender.Identifier;
            this.Damage = damage;
        }

        public EntityIdentifier AttackerIdentifier { get; private set; }

        public EntityIdentifier DefenderIdentifier { get; private set; }

        public int Damage { get; private set; }

    }

    public class DestroyEntityEvent : GameEvent
    {
        public DestroyEntityEvent(GameEntity e)
            : base(GameEventType.Destroy)
        {
            this.Identifier = e.Identifier;
        }

        public EntityIdentifier Identifier { get; private set; }
    }

    public class UpdateMovementEvent : GameEvent
    {
        public UpdateMovementEvent(GameEntity entity)
            : this(entity.Identifier, entity.Movements)
        {
        }

        public UpdateMovementEvent(EntityIdentifier id, MovementStruct movements)
            : base(GameEventType.UpdateMovement)
        {
            this.Identifier = id;
            this.Movements = movements;
        }

        public EntityIdentifier Identifier { get; private set; }

        public MovementStruct Movements { get; private set; }
    }

    public enum GameEventType
    {
        NotSet,

        Create,
        Destroy,
        UpdateMovement,
        Damage,
        ApplyStatus,

        UseAbility,
    }
}
