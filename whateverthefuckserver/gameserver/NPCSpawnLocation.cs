using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;

namespace whateverthefuckserver.gameserver
{
    class NPCSpawnLocation
    {
        public NPCSpawnLocation(GameCoordinate location, params NPCCreationArguments.Types[] spawnableTypes)
        {
            Location = location;
            SpawnableTypes = spawnableTypes;
        }

        private GameCoordinate Location { get; }

        private NPCCreationArguments.Types[] SpawnableTypes { get; }

        private int level = 1;

        public CreateEntityEvent SpawnNPC(GameState gameState)
        {
            // @fix this creates a GameEntity then clones it into a createEntityEvent then recreates it from there
            var mob = (NPC)gameState.EntityGenerator.GenerateEntity(EntityType.NPC, new NPCCreationArguments(NPCCreationArguments.Types.Dog, level++, 0));
            mob.Info.GameLocation = new GameCoordinate(Location);
            var createEntityEvent = new CreateEntityEvent(mob);
            return createEntityEvent;
        }
    }
}
