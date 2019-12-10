using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public abstract class GameEvent
    {
        public static GameEvent DecodeWithEventType(GameEventType type, byte[] body)
        {
            switch (type)
            {
                case GameEventType.Move:
                {
                    return new MoveEntityEvent(body);
                }

                case GameEventType.Create:
                {
                    return new CreateEntityEvent(body);
                }

                default: throw new Exception();
            }
        }

        public GameEventType Type { get; protected set; }
        public abstract byte[] ToBytes();
    }

    public class MoveEntityEvent : GameEvent
    {
        public int Id { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public MoveEntityEvent(int id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
            Type = GameEventType.Move;
        }

        public MoveEntityEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.Move;

            Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));
            X = BitConverter.ToSingle(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));
            Y = BitConverter.ToSingle(bs.ToArray(), 0);
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).Concat(
                BitConverter.GetBytes(X).Concat(
                BitConverter.GetBytes(Y)))
                .ToArray();
        }
    }

    public class CreateEntityEvent : GameEvent
    {
        public int Id { get; private set; }
        public EntityType EntityType { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public CreateEntityEvent(GameEntity e)
        {
            Type = GameEventType.Create;
            
            Id = e.Identifier.Id;
            EntityType = e.EntityType;
            X = e.Location.X;
            Y = e.Location.Y;
        }

        public CreateEntityEvent(IEnumerable<byte> bs)
        {
            Type = GameEventType.Create;

            Id = BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            EntityType = (EntityType)BitConverter.ToInt32(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            X = BitConverter.ToSingle(bs.ToArray(), 0);
            bs = bs.Skip(sizeof(int));

            Y = BitConverter.ToSingle(bs.ToArray(), 0);
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Id).Concat(
                BitConverter.GetBytes((int)EntityType).Concat(
                BitConverter.GetBytes(X).Concat(
                BitConverter.GetBytes(Y))))
                .ToArray();
        }
    }


    public enum GameEventType
    {
        NotSet,

        Move,
        Create,
        Destroy,
    }
}
