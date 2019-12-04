using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    /// <summary>
    /// Doors are defined as holes where there are no blocks.
    /// </summary>
    class Door : GameEntity
    {
        Door(EntityIdentifier identifier) : base(ControlInfo.NoControl, identifier)
        {
        }
    }
}
