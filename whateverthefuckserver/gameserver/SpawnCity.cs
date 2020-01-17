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
    class SpawnCity
    {
        private GameState GameState { get; set; }
        private Action<IEnumerable<GameEvent>> Publish { get; }

        private List<NPCSpawnLocation> SpawnLocations { get; } = new List<NPCSpawnLocation>();

        public SpawnCity(GameState gameState, Action<IEnumerable<GameEvent>> publish)
        {
            GameState = gameState;
            Publish = publish;


            SpawnLocations.Add(new NPCSpawnLocation(new GameCoordinate(-1, -1), 1, 5, NPCCreationArguments.Types.Dog));
            SpawnLocations.Add(new NPCSpawnLocation(new GameCoordinate(-1, -2), 5, 10, NPCCreationArguments.Types.Dog));
            SpawnLocations.Add(new NPCSpawnLocation(new GameCoordinate(-1, -3), 10, 15, NPCCreationArguments.Types.Dog));
            SpawnLocations.Add(new NPCSpawnLocation(new GameCoordinate(-1, -4), 15, 20, NPCCreationArguments.Types.Dog));
        }

        private void PublishArray(params GameEvent[] es)
        {
            Publish(es);
        }

        public CreateEntityEvent SpawnHero()
        {
            var rt = new CreateEntityEvent(GameState.EntityGenerator.GenerateEntity(EntityType.PC, new PCCreationArguments()));
            PublishArray(rt);
            return rt;
        }

        public void SpawnNPCs()
        {
            foreach (var spawnLocation in SpawnLocations)
            {
                SpawnNPC(spawnLocation);
            }
        }

        private void SpawnNPC(NPCSpawnLocation spawnLocation)
        {
            var createEntityEvent = spawnLocation.SpawnNPC(GameState);

            createEntityEvent.OnEntityCreated += (entity, gameState) =>
            {
                Brain brain = new Brain(entity);
                entity.OnDeath += (e, gs) => OnDeath(entity, brain, spawnLocation);
                entity.OnStep += (e, gs) => UseBrain(brain, e, gs);
            };

            PublishArray(createEntityEvent);
        }


        private void OnDeath(GameEntity entity, Brain brain, NPCSpawnLocation spawnLocation)
        {
            Program.GameServer.SpawnLootForPlayer(entity.Info.Identifier, brain.Tagger);
            SpawnNPC(spawnLocation);
        }

        private void UseBrain(Brain brain, GameEntity entity, GameState gameState)
        {
            var action = brain.Use(entity, gameState);

            if (action != null)
            {
                PublishArray(action);
            }
        }

        public void SpawnWorld()
        {
            var house = GameState.EntityGenerator.GenerateHouse(5, 5).Select(e => new CreateEntityEvent(e));

            Publish(house);
        }
    }
}
