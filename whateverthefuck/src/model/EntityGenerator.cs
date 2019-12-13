namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using whateverthefuck.src.model.entities;

    /// <summary>
    /// Responsible for generating GameEntities from a given EntityType and CreationArgs.
    /// </summary>
    public class EntityGenerator
    {
        public EntityGenerator(IdentifierGenerator idGenerator)
        {
            this.IdGenerator = idGenerator;
        }

        private static GameCoordinate GridSize { get; } = new GameCoordinate(0.1f, 0.1f);

        private IdentifierGenerator IdGenerator { get; }

        public GameEntity GenerateEntity(EntityType type, EntityIdentifier identifier, CreationArgs args)
        {
            switch (type)
            {
                case EntityType.Block:
                    {
                        return new Block(identifier, args);
                    }

                case EntityType.NPC:
                    {
                        return new NPC(identifier, args);
                    }

                case EntityType.PlayerCharacter:
                    {
                        return new PlayerCharacter(identifier, args);
                    }

                case EntityType.Door:
                    {
                        return new Door(identifier, args);
                    }

                case EntityType.Floor:
                    {
                        return new Floor(identifier, args);
                    }

                case EntityType.Projectile:
                    {
                        return new Projectile(identifier, args);
                    }

                case EntityType.Loot:
                    {
                        return new Loot(identifier, args);
                    }

                default: throw new Exception();
            }
        }

        public GameEntity GenerateEntity(CreateEntityEvent e)
        {
            var rt = this.GenerateEntity(e.EntityType, new EntityIdentifier(e.Id), e.CreationArgs);
            rt.Location = new GameCoordinate(e.X, e.Y);
            rt.CurrentHealth = e.CurrentHealth;
            rt.MaxHealth = e.MaxHealth;
            return rt;
        }

        public GameEntity GenerateEntity(EntityType e, CreationArgs a)
        {
            return this.GenerateEntity(e, this.IdGenerator.GenerateNextIdentifier(), a);
        }

        public IEnumerable<GameEntity> GenerateHouse(int xorg, int yorg)
        {
            var dca = new DoorCreationArgs(DoorCreationArgs.Types.Wood);
            var fca = new FloorCreationArgs(FloorCreationArgs.Types.Stone);
            var bca = new BlockCreationArgs(BlockCreationArgs.Types.Stone);

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
                        e = this.GenerateEntity(EntityType.Door, dca);
                    }
                    else if (x == xorg || x == xorg + width - 1)
                    {
                        e = this.GenerateEntity(EntityType.Block, bca);
                    }
                    else if (y == yorg || y == yorg + height - 1)
                    {
                        e = this.GenerateEntity(EntityType.Block, bca);
                    }
                    else
                    {
                        e = this.GenerateEntity(EntityType.Floor, fca);
                    }

                    e.Location = new GameCoordinate(x * GridSize.X, y * GridSize.Y);

                    rt.Add(e);
                }
            }

            return rt;
        }
    }

    /// <summary>
    /// Uniquely identifies a GameEntity within a GameState.
    /// </summary>
    public class IdentifierGenerator
    {
        private int idCounter = 0;

        public EntityIdentifier GenerateNextIdentifier()
        {
            return new EntityIdentifier(this.idCounter++);
        }
    }
}
