using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.model
{
    public class EntityGenerator
    {
        private static GameCoordinate GridSize = new GameCoordinate(0.1f, 0.1f);

        private IdentifierGenerator IdGenerator;

        public EntityGenerator(IdentifierGenerator idGenerator)
        {
            IdGenerator = idGenerator;
        }


        public GameEntity GenerateEntity(EntityType type)
        {
            return GenerateEntity(type, IdGenerator.GenerateNextIdentifier());
        }

        public GameEntity GenerateEntity(EntityType type, EntityIdentifier identifier)
        {
            switch (type)
            {
                case EntityType.Block:
                {
                    return new Block(identifier, Color.Teal);
                }

                case EntityType.NPC:
                {
                    return new NPC(identifier);
                }

                case EntityType.PlayerCharacter:
                {
                    return new PlayerCharacter(identifier);
                }

                case EntityType.Door:
                {
                    return new Door(identifier);
                }

                case EntityType.Floor:
                {
                    return new Floor(identifier);
                }

                case EntityType.Projectile:
                {
                    return new Projectile(identifier);
                }

                case EntityType.Loot:
                {
                    return new Loot(identifier);
                }

                default: throw new Exception();
            }
        }

        public GameEntity GenerateEntity(CreateEntityEvent e)
        {
            var rt = GenerateEntity(e.EntityType, new EntityIdentifier(e.Id));
            rt.Location = new GameCoordinate(e.X, e.Y);
            rt.CurrentHealth = e.CurrentHealth;
            rt.MaxHealth = e.MaxHealth;
            return rt;
        }

        public IEnumerable<GameEntity> GenerateHouse(int xorg, int yorg)
        {
            List<GameEntity> rt = new List<GameEntity>();

            int width = 10;
            int height = 10;

            for (int x = xorg; x < xorg + 10; x++)
            {
                for (int y = yorg; y < yorg + 10; y++)
                {
                    GameEntity e;
                    if (x == xorg + 1 && y == yorg)
                    {
                        e = new Door(IdGenerator.GenerateNextIdentifier());
                    }
                    else if (x == xorg || x == xorg + width - 1)
                    {
                        e = new Block(IdGenerator.GenerateNextIdentifier(), Color.Red);
                    }
                    else if (y == yorg || y == yorg + height -1)
                    {
                        e = new Block(IdGenerator.GenerateNextIdentifier(), Color.Red);
                    }
                    else
                    {
                        e = new Floor(IdGenerator.GenerateNextIdentifier());
                    }

                    e.Location = new GameCoordinate(x * GridSize.X, y * GridSize.Y);

                    rt.Add(e);
                }

            }
            return rt;
        }

    }


    public class IdentifierGenerator
    {
        private int IdCounter = 0;

        public EntityIdentifier GenerateNextIdentifier()
        {
            return new EntityIdentifier(IdCounter++);
        }
    }
}
