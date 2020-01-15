﻿namespace whateverthefuck.src.model
{
    using System;
    using whateverthefuck.src.util;

    /// <summary>
    /// Represents an event which alters a GameState.
    /// </summary>
    public abstract class GameEvent : IEncodable
    {
        protected GameEvent(GameEventType type)
        {
            this.Type = type;
        }

        public GameEventType Type { get; protected set; }

        public static GameEvent FromType(GameEventType type)
        {
            switch (type)
            {
                case GameEventType.UpdateMovement:
                {
                    return new UpdateMovementEvent();
                }

                case GameEventType.UseAbility:
                {
                    return new BeginCastAbilityEvent();
                }

                case GameEventType.Create:
                {
                    return new CreateEntityEvent();
                }

                default:
                {
                    throw new Exception("Unexpectedly received a " + type);
                }
            }
        }

        public virtual void Encode(WhateverEncoder encoder)
        {
            Logging.Log(this.Type + " doesn't encode", Logging.LoggingLevel.Error);
        }

        public virtual void Decode(WhateverDecoder decoder)
        {
            Logging.Log(this.Type + "doesn't decode", Logging.LoggingLevel.Error);
        }
    }

    public class ApplyStatusEvent : GameEvent
    {
        public ApplyStatusEvent(GameEntity entity, Status appliedStatus)
            : base(GameEventType.ApplyStatus)
        {
            this.EntityIdentifier = entity.Info.Identifier;
            this.Status = appliedStatus;
        }

        public EntityIdentifier EntityIdentifier { get; }

        public Status Status { get; }
    }

    public class BeginCastAbilityEvent : GameEvent
    {
        public BeginCastAbilityEvent()
            : base(GameEventType.UseAbility)
        {
        }

        public BeginCastAbilityEvent(GameEntity caster, GameEntity target, Ability ability)
            : base(GameEventType.UseAbility)
        {
            this.CasterIdentifier = caster.Info.Identifier;
            this.AbilityType = ability.AbilityType;
            this.TargetIdentifier = target.Info.Identifier;
        }

        // @consider changing this to the index of the ability within the casters ability array
        public AbilityType AbilityType { get; private set; }

        public EntityIdentifier CasterIdentifier { get; private set; }

        public EntityIdentifier TargetIdentifier { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.AbilityType);
            encoder.Encode(this.CasterIdentifier.Id);
            encoder.Encode(this.TargetIdentifier.Id);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.AbilityType = (AbilityType)decoder.DecodeInt();
            this.CasterIdentifier = new EntityIdentifier(decoder.DecodeInt());
            this.TargetIdentifier = new EntityIdentifier(decoder.DecodeInt());
        }
    }

    public class EndCastAbility : GameEvent
    {
        public EndCastAbility(EntityIdentifier caster, EntityIdentifier target, Ability ability)
            : base(GameEventType.UseAbility)
        {
            this.AbilityType = ability.AbilityType;
            this.CasterIdentifier = caster;
            this.TargetIdentifier = target;
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
            this.Id = e.Info.Identifier;
            this.EntityType = e.Info.EntityType;
            this.X = e.Info.GameLocation.X;
            this.Y = e.Info.GameLocation.Y;
            this.CurrentHealth = e.Status.BaseStats.Health;
            this.MaxHealth = e.Status.BaseStats.MaxHealth;
            this.CreationArgs = e.Info.CreationArgs;
        }

        public CreateEntityEvent(EntityIdentifier id, EntityType entityType, float x, float y, int currentHealth, int maxHealth, CreationArguments creationArgs)
            : base(GameEventType.Create)
        {
            this.Id = id;
            this.EntityType = entityType;
            this.X = x;
            this.Y = y;
            this.CurrentHealth = currentHealth;
            this.MaxHealth = maxHealth;
            this.CreationArgs = creationArgs;
        }

        public CreateEntityEvent()
            : base(GameEventType.Create)
        {
        }

        public EntityIdentifier Id { get; private set; }

        public EntityType EntityType { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public int CurrentHealth { get; private set; }

        public int MaxHealth { get; private set; }

        public CreationArguments CreationArgs { get; private set; }

        public Action<GameEntity, GameState> OnEntityCreatedCallback { get; set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.Id.Id);
            encoder.Encode((int)this.EntityType);
            encoder.Encode(this.X);
            encoder.Encode(this.Y);
            encoder.Encode(this.MaxHealth);
            encoder.Encode(this.CurrentHealth);
            this.CreationArgs.Encode(encoder);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Id = new EntityIdentifier(decoder.DecodeInt());
            this.EntityType = (EntityType)decoder.DecodeInt();
            this.X = decoder.DecodeFloat();
            this.Y = decoder.DecodeFloat();
            this.MaxHealth = decoder.DecodeInt();
            this.CurrentHealth = decoder.DecodeInt();
            this.CreationArgs = CreationArguments.FromEntityType(this.EntityType);
            this.CreationArgs.Decode(decoder);
        }
    }

    public class DealDamageEvent : GameEvent
    {
        public DealDamageEvent(GameEntity attacker, GameEntity defender, int damage)
            : base(GameEventType.Damage)
        {
            this.Type = GameEventType.Damage;

            this.AttackerIdentifier = attacker.Info.Identifier;
            this.DefenderIdentifier = defender.Info.Identifier;
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
            this.Identifier = e.Info.Identifier;
        }

        public EntityIdentifier Identifier { get; private set; }
    }

    public class UpdateMovementEvent : GameEvent
    {
        public UpdateMovementEvent()
            : base(GameEventType.UpdateMovement)
        {
        }

        public UpdateMovementEvent(GameEntity entity)
            : this(entity.Info.Identifier, entity.Movements)
        {
        }

        public UpdateMovementEvent(EntityIdentifier id, EntityMovement movements)
            : base(GameEventType.UpdateMovement)
        {
            this.Identifier = id;
            this.Movements = movements;
        }

        public EntityIdentifier Identifier { get; private set; }

        public EntityMovement Movements { get; private set; }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(this.Identifier.Id);
            this.Movements.Encode(encoder);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Identifier = new EntityIdentifier(decoder.DecodeInt());
            var movements = new EntityMovement();
            movements.Decode(decoder);
            this.Movements = movements;
        }
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
