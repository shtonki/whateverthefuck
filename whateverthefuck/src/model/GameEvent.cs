namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an event which alters a GameState.
    /// </summary>
    public abstract class GameEvent
    {
        public GameEventType Type { get; protected set; }

        public static GameEvent DecodeWithEventType(GameEventType type, byte[] body)
        {
            switch (type)
            {
                case GameEventType.Create:
                {
                    return new CreateEntityEvent(body);
                }

                case GameEventType.Destroy:
                {
                    return new DestroyEntityEvent(body);
                }

                case GameEventType.Control:
                {
                    return new UpdateMovementEvent(body);
                }

                case GameEventType.UseAbility:
                {
                    return new BeginCastAbility(body);
                }

                default: throw new Exception();
            }
        }

        public abstract byte[] ToBytes();
    }

    public class BeginCastAbility : GameEvent
    {
        public BeginCastAbility(GameEntity caster, GameEntity target, Ability ability)
        {
            this.Type = GameEventType.UseAbility;

            this.CasterId = caster.Identifier.Id;
            this.AbilityType = ability.AbilityType;
            this.TargetId = target.Identifier.Id;
        }

        public BeginCastAbility(IEnumerable<byte> bs)
        {
            this.Type = GameEventType.UseAbility;

            this.CasterId = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            this.AbilityType = (AbilityType)BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            this.TargetId = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));
        }

        public int CasterId { get; private set; }

        public AbilityType AbilityType { get; private set; }

        public int TargetId { get; private set; }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(this.CasterId).Concat(
                BitConverter.GetBytes((int)this.AbilityType).Concat(
                BitConverter.GetBytes(this.TargetId)))
                .ToArray();
        }
    }

    public class EndCastAbility : GameEvent
    {
        public EndCastAbility(GameEntity caster, GameEntity target, Ability ability)
        {
            this.Type = GameEventType.UseAbility;

            this.CasterId = caster.Identifier.Id;
            this.AbilityType = ability.AbilityType;
            this.TargetId = target.Identifier.Id;
        }

        public int CasterId { get; private set; }

        public AbilityType AbilityType { get; private set; }

        public int TargetId { get; private set; }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException(
                "You shouldn't be sending this over the network.");
        }
    }

    public class CreateEntityEvent : GameEvent
    {


        public CreateEntityEvent(GameEntity e)
        {
            this.Type = GameEventType.Create;

            this.Id = e.Identifier.Id;
            this.EntityType = e.EntityType;
            this.X = e.Location.X;
            this.Y = e.Location.Y;
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

        public CreateEntityEvent(IEnumerable<byte> bs)
        {
            this.Type = GameEventType.Create;

            this.Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            this.EntityType = (EntityType)BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            this.X = BitConverter.ToSingle(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(float));

            this.Y = BitConverter.ToSingle(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(float));

            this.CurrentHealth = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            this.MaxHealth = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            this.CreationArgs = new CreationArgs(BitConverter.ToUInt64(bs.ToArray(), 0));
            bs = bs.Skip(sizeof(long));
        }

        public CreateEntityEvent(EntityIdentifier id, EntityType entityType, float x, float y, int currentHealth, int maxHealth, CreationArgs creationArgs)
        {
            Id = id.Id;
            EntityType = entityType;
            X = x;
            Y = y;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            CreationArgs = creationArgs;
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

        public Action<GameEntity> OnStepCallback { get; set; }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(this.Id).Concat(
                BitConverter.GetBytes((int)this.EntityType).Concat(
                BitConverter.GetBytes(this.X).Concat(
                BitConverter.GetBytes(this.Y).Concat(
                BitConverter.GetBytes(this.CurrentHealth).Concat(
                BitConverter.GetBytes(this.MaxHealth).Concat(
                BitConverter.GetBytes(this.CreationArgs.Value))))))).ToArray();
        }
    }

    public class DealDamageEvent : GameEvent
    {
        public DealDamageEvent(int attackerId, int defenderId, int damage)
        {
            this.AttackerId = attackerId;
            this.DefenderId = defenderId;
            this.Damage = damage;
        }

        public DealDamageEvent(GameEntity attacker, GameEntity defender, int damage)
        {
            this.Type = GameEventType.Damage;

            this.AttackerId = attacker.Identifier.Id;
            this.DefenderId = defender.Identifier.Id;
            this.Damage = damage;
        }

        public DealDamageEvent(IEnumerable<byte> bs)
        {
            this.Type = GameEventType.Damage;
            int bytec = 0;

            this.AttackerId = BitConverter.ToInt32(bs.ToArray(), bytec);
            bytec += sizeof(int);

            this.DefenderId = BitConverter.ToInt32(bs.ToArray(), bytec);
            bytec += sizeof(int);

            this.Damage = BitConverter.ToInt32(bs.ToArray(), bytec);
            bytec += sizeof(int);
        }

        public int AttackerId { get; private set; }

        public int DefenderId { get; private set; }

        public int Damage { get; private set; }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(this.AttackerId).Concat(
                BitConverter.GetBytes(this.DefenderId).Concat(
                BitConverter.GetBytes(this.Damage))).ToArray();
        }
    }

    public class DestroyEntityEvent : GameEvent
    {
        public DestroyEntityEvent(GameEntity e)
        {
            this.Type = GameEventType.Destroy;

            this.Id = e.Identifier.Id;
        }

        public DestroyEntityEvent(IEnumerable<byte> bs)
        {
            this.Type = GameEventType.Destroy;

            this.Id = BitConverter.ToInt32(bs.ToArray(), 0);
        }

        public int Id { get; private set; }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(this.Id).ToArray();
        }
    }

    public class UpdateMovementEvent : GameEvent
    {
        public UpdateMovementEvent(GameEntity entity)
            : this(entity.Identifier.Id, entity.Movements)
        {
        }

        public UpdateMovementEvent(int id, MovementStruct movements)
        {
            this.Type = GameEventType.Control;

            this.Id = id;
            this.Movements = movements;
        }

        public UpdateMovementEvent(IEnumerable<byte> bs)
        {
            this.Type = GameEventType.Control;

            this.Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int)).ToArray();
            this.Movements = MovementStruct.Decode(bs.ToArray());
        }

        public int Id { get; private set; }

        public MovementStruct Movements { get; private set; }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(this.Id).Concat(
                this.Movements.Encode())
                .ToArray();
        }
    }

    public enum GameEventType
    {
        NotSet,

        Create,
        Destroy,
        Control,
        Damage,

        UseAbility,
    }
}
