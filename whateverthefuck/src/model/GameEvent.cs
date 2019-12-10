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

        public UseAbilityEvent(GameEntity caster, GameEntity Target, Ability ability)
        {
            Type = GameEventType.UseAbility;
            
            Id = caster.Identifier.Id;
            AbilityType = ability.AbilityType;
            TargetId = Target.Identifier.Id;
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

        public CreateEntityEvent(GameEntity e)
        {
            Type = GameEventType.Create;
            
            Id = e.Identifier.Id;
            EntityType = e.EntityType;
            X = e.Location.X;
            Y = e.Location.Y;
            CurrentHealth = e.CurrentHealth;
            MaxHealth = e.MaxHealth;
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
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).Concat(
                BitConverter.GetBytes((int)EntityType).Concat(
                BitConverter.GetBytes(X).Concat(
                BitConverter.GetBytes(Y).Concat(
                BitConverter.GetBytes(CurrentHealth).Concat(
                BitConverter.GetBytes(MaxHealth)
                ))))).ToArray();
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

        UseAbility,
    }
}
