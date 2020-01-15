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

        public void SpawnMob()
        {
            var mob = (NPC)GameState.EntityGenerator.GenerateEntity(EntityType.NPC, new NPCCreationArguments(NPCCreationArguments.Types.Dog));
            mob.Info.GameLocation = new GameCoordinate(-0.5f, RNG.BetweenZeroAndOne());
            var rt = new CreateEntityEvent(mob);
            rt.OnDeathCallback = (idiot, killer) => Program.GameServer.SpawnLootForPlayer(idiot, killer);

            Brain brain = new Brain();
            rt.OnStepCallback = (entity, gameState) => UseBrain(brain, entity as NPC, gameState);

            PublishArray(rt);
        }

        private void UseBrain(Brain brain, NPC npc, GameState gameState)
        {
            var action = brain.Use(npc, gameState);

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
