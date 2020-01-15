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

        public SpawnCity(GameState gameState, Action<IEnumerable<GameEvent>> publish)
        {
            GameState = gameState;
            Publish = publish;
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

        public void SpawnNPC()
        {
            // @fix this creates a GameEntity then clones it into a createEntityEvent then recreates it from there
            var mob = (NPC)GameState.EntityGenerator.GenerateEntity(EntityType.NPC, new NPCCreationArguments(NPCCreationArguments.Types.Dog));
            mob.Info.GameLocation = new GameCoordinate(-0.5f, RNG.BetweenZeroAndOne());
            var createEntityEvent = new CreateEntityEvent(mob);


            createEntityEvent.OnEntityCreatedCallback = (entity, gameState) =>
            {
                Brain brain = new Brain(entity);
                entity.OnDeath += (e, gs) => OnDeath(entity, brain);
                entity.OnStep += (e, gs) => UseBrain(brain, e, gs);
            };

            PublishArray(createEntityEvent);
        }


        private void OnDeath(GameEntity entity, Brain brain)
        {
            Program.GameServer.SpawnLootForPlayer(entity.Info.Identifier, brain.Tagger);
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
