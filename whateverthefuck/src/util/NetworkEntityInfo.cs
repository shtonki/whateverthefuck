using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.util
{
    public class NetworkEntityInfo
    {
        public GameCoordinate Location { get; }
        public GameCoordinate Size { get; }
        public EntityIdentifier Identifier { get; }

        public NetworkEntityInfo(GameEntity entity)
        {
            if (!(entity.Location is GameCoordinate)) { Logger.Log("We have encountered a GameEntity which has a Location which isn't a GameCoordiate. This would be where we panic."); }
            
            Location = (GameCoordinate)entity.Location;
            Size = entity.Size;
            Identifier = entity.Identifier;
        }
    }
}
