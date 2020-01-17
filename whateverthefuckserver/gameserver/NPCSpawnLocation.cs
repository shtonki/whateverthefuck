using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;

namespace whateverthefuckserver.gameserver
{
    class NPCSpawnLocation
    {
        public NPCSpawnLocation(GameCoordinate location, int levelMin, int levelMax, params NPCCreationArguments.Types[] spawnableTypes)
        {
            Location = location;
            SpawnableTypes = spawnableTypes;
            LevelMin = levelMin;
            LevelMax = levelMax;
        }

        private GameCoordinate Location { get; }

        private NPCCreationArguments.Types[] SpawnableTypes { get; }

        private int LevelMin { get; }

        private int LevelMax { get; }

        public CreateEntityEvent SpawnNPC(GameState gameState)
        {
            // @fix this creates a GameEntity then clones it into a createEntityEvent then recreates it from there
            var mob = (NPC)gameState.EntityGenerator.GenerateEntity(EntityType.NPC, new NPCCreationArguments(NPCCreationArguments.Types.Dog, GetRandomLevel(), 0));
            mob.Info.GameLocation = new GameCoordinate(Location);
            var createEntityEvent = new CreateEntityEvent(mob);
            return createEntityEvent;
        }

        private int GetRandomLevel()
        {
            return RNG.IntegerBetween(LevelMin, LevelMax + 1);
        }
    }
}
