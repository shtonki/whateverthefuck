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
        public GameCoordinate Size { get; }
        public EntityIdentifier Identifier { get; }

        public NetworkEntityInfo(GameEntity entity)
        {
            if (!(entity.Location is GameCoordinate)) { Logging.Log("We have encountered a GameEntity which has a Location which isn't a GameCoordiate. This would be where we panic."); }

            Location = (GameCoordinate)entity.Location;
            Size = entity.Size;
            Identifier = entity.Identifier;
        }
    }
}
