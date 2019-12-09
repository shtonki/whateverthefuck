using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model.entities
{
    /// <summary>
    /// Doors are defined as holes where there are no blocks.
    /// </summary>
    public class Door : GameEntity
    {
        public Door(EntityIdentifier identifier) : base(identifier, EntityType.Door)
        {
            Collidable = false;
            Height = 15;
        }
    }
}
