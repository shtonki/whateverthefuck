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

        public GameEntity GenerateBlock(GameCoordinate size, GameCoordinate location)
        {
            Block b = new Block(IdGenerator.GenerateNextIdentifier(), Color.Teal);
            b.Size = size;
            b.Location = location;
            return b;
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
