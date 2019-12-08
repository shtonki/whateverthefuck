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
        private IdentifierGenerator IdGenerator;

        public EntityGenerator(IdentifierGenerator idGenerator)
        {
            IdGenerator = idGenerator;
        }

        public GameEntity GenerateEntity(EntityType type)
        {
            switch (type)
            {
                case EntityType.Block:
                {
                    return new Block(IdGenerator.GenerateNextIdentifier(), Color.Teal);
                }

                case EntityType.NPC:
                {
                    return new NPC(IdGenerator.GenerateNextIdentifier());
                }

                case EntityType.PlayerCharacter:
                {
                    return new PlayerCharacter(IdGenerator.GenerateNextIdentifier());
                }

                case EntityType.Door:
                {
                    return new Door(IdGenerator.GenerateNextIdentifier());
                }

                default: throw new Exception();
            }
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
