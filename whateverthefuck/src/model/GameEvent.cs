using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    /// <summary>
    /// Represents an event which alters a GameState.
    /// </summary>
    public abstract class GameEvent
    {
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
                    return new UseAbilityEvent(body);
                }

                default: throw new Exception();
            }
        }

        public GameEventType Type { get; protected set; }

        public abstract byte[] ToBytes();
    }

    public class UseAbilityEvent : GameEvent
    {
        public int Id { get; private set; }

        public Abilities AbilityType { get; private set; } 

        public int TargetId { get; private set; }

        public UseAbilityEvent(GameEntity caster, GameEntity target, Ability ability)
        {
            Type = GameEventType.UseAbility;
            
            Id = caster.Identifier.Id;
            AbilityType = ability.AbilityType;
            TargetId = target.Identifier.Id;
        }

        public UseAbilityEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.UseAbility;

            Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int)); 

            AbilityType = (Abilities)BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int)); 

            TargetId = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).Concat(
                BitConverter.GetBytes((int)AbilityType).Concat(
                BitConverter.GetBytes(TargetId)))
                .ToArray();
        }
    }

    public class CreateEntityEvent : GameEvent
    {
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

        public CreateEntityEvent(GameEntity e)
        {
            Type = GameEventType.Create;
            
            Id = e.Identifier.Id;
            EntityType = e.EntityType;
            X = e.Location.X;
            Y = e.Location.Y;
            CurrentHealth = e.CurrentHealth;
            MaxHealth = e.MaxHealth;

            if (e.CreationArgs == null)
            {
                CreationArgs = new CreationArgs(0);
            }
            else
            { 
                CreationArgs = e.CreationArgs;
            }
        }

        public CreateEntityEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.Create;

            Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            EntityType = (EntityType)BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            X = BitConverter.ToSingle(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(float));

            Y = BitConverter.ToSingle(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(float));

            CurrentHealth = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            MaxHealth = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            CreationArgs = new CreationArgs(BitConverter.ToUInt64(bs.ToArray(), 0));
            bs = bs.Skip(sizeof(long));
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).Concat(
                BitConverter.GetBytes((int)EntityType).Concat(
                BitConverter.GetBytes(X).Concat(
                BitConverter.GetBytes(Y).Concat(
                BitConverter.GetBytes(CurrentHealth).Concat(
                BitConverter.GetBytes(MaxHealth).Concat(
                BitConverter.GetBytes(CreationArgs.Value)
                )))))).ToArray();
        }
    }

    public class DealDamageEvent : GameEvent
    {
        public int AttackerId { get; private set; }

        public int DefenderId { get; private set; }

        public int Damage { get; private set; }

        public DealDamageEvent(int attackerId, int defenderId, int damage)
        {
            AttackerId = attackerId;
            DefenderId = defenderId;
            Damage = damage;
        }

        public DealDamageEvent(GameEntity attacker, GameEntity defender, int damage)
        {
            Type = GameEventType.Damage;

            AttackerId = attacker.Identifier.Id;
            DefenderId = defender.Identifier.Id;
            Damage = damage;
        }

        public DealDamageEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.Damage;
            int bytec = 0;

            AttackerId = BitConverter.ToInt32(bs.ToArray(), bytec);
            bytec += sizeof(int);

            DefenderId = BitConverter.ToInt32(bs.ToArray(), bytec);
            bytec += sizeof(int);

            Damage = BitConverter.ToInt32(bs.ToArray(), bytec);
            bytec += sizeof(int);
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(AttackerId).Concat(
                BitConverter.GetBytes(DefenderId).Concat(
                BitConverter.GetBytes(Damage)  
                    )).ToArray();
        }
    }

    public class DestroyEntityEvent : GameEvent
    {
        public int Id { get; private set; }

        public DestroyEntityEvent(GameEntity e)
        {
            Type = GameEventType.Destroy;

            Id = e.Identifier.Id;
        }

        public DestroyEntityEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.Destroy;

            Id = BitConverter.ToInt32(bs.ToArray(), 0);
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).ToArray();
        }
    }

    public class UpdateMovementEvent : GameEvent
    {
        public int Id { get; private set; }

        public MovementStruct Movements { get; private set; }

        public UpdateMovementEvent(GameEntity entity) : this(entity.Identifier.Id, entity.Movements)
        {
        }

        public UpdateMovementEvent(int id, MovementStruct movements)
        {
            Type = GameEventType.Control;

            Id = id;
            Movements = movements;
        }

        public UpdateMovementEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.Control;

            Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int)).ToArray();
            Movements = MovementStruct.Decode(bs.ToArray());
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).Concat(
                Movements.Encode())
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
