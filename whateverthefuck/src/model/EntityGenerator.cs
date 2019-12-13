using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;

namespace whateverthefuck.src.model
{
    /// <summary>
    /// Responsible for generating GameEntities from a given EntityType and CreationArgs.
    /// </summary>
    public class EntityGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityGenerator"/> class.
        /// </summary>
        /// <param name="idGenerator">The IdentifierGenerator used to assign EntityIdentifiers to generated GameEntities.</param>
        public EntityGenerator(IdentifierGenerator idGenerator)
        {
            this.IdGenerator = idGenerator;
        }

        private static GameCoordinate GridSize { get; } = new GameCoordinate(0.1f, 0.1f);

        private IdentifierGenerator IdGenerator { get; }

        /// <summary>
        /// Generates a GameEntity.
        /// </summary>
        /// <param name="type">EntityType of the GameEntity.</param>
        /// <param name="identifier">EntityIdentifier of the GameEntity.</param>
        /// <param name="args">CreationArgs used to create the GameEntity.</param>
        /// <returns>The created GameEntity.</returns>
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

        /// <summary>
        /// Generates a GameEntity from a CreateEntityEvent.
        /// </summary>
        /// <param name="e">The CreateEntityEvent containing the creation inforomation.</param>
        /// <returns>The created GameEntity.</returns>
        public GameEntity GenerateEntity(CreateEntityEvent e)
        {
            var rt = this.GenerateEntity(e.EntityType, new EntityIdentifier(e.Id), e.CreationArgs);
            rt.Location = new GameCoordinate(e.X, e.Y);
            rt.CurrentHealth = e.CurrentHealth;
            rt.MaxHealth = e.MaxHealth;
            return rt;
        }

        /// <summary>
        /// Generates a GameEntity.
        /// </summary>
        /// <param name="e">The EntityType of the GameEntity.</param>
        /// <param name="a">The CreationArgs used to create the GameEntity.</param>
        /// <returns>The created GameEntity.</returns>
        public GameEntity GenerateEntity(EntityType e, CreationArgs a)
        {
            return this.GenerateEntity(e, this.IdGenerator.GenerateNextIdentifier(), a);
        }

        /// <summary>
        /// Generates a house consisting of Blocks, Floors, and Doors.
        /// </summary>
        /// <param name="xorg">Grid index of houses X starting location.</param>
        /// <param name="yorg">Grid index of houses Y starting location.</param>
        /// <returns>The GameEntities making up the created house.</returns>
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


    public class IdentifierGenerator
    {
        private int IdCounter = 0;

        public EntityIdentifier GenerateNextIdentifier()
        {
            return new EntityIdentifier(this.IdCounter++);
        }
    }
}
