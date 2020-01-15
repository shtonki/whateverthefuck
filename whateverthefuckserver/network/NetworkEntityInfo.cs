using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuckserver.network
{
    public class NetworkEntityInfo
    {
        public GameCoordinate Location { get; }
        public EntityIdentifier Identifier { get; }

        public NetworkEntityInfo(GameEntity entity)
        {
            Location = entity.Info.GameLocation;
            Identifier = entity.Info.Identifier;
        }
    }
}
