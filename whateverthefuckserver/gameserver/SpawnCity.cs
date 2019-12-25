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
            var rt = new CreateEntityEvent(GameState.EntityGenerator.GenerateEntity(EntityType.PlayerCharacter, new CreationArgs(0)));
            PublishArray(rt);
            return rt;
        }

        public void SpawnMob()
        {
            var mob = (NPC)GameState.EntityGenerator.GenerateEntity(EntityType.NPC, new CreationArgs(0));
            mob.Location = new GameCoordinate(-0.5f, RNG.BetweenZeroAndOne());
            var rt = new CreateEntityEvent(mob);
            rt.OnDeathCallback = (idiot, killer) => Program.GameServer.SpawnLootForPlayer(idiot, killer);

            Brain brain = new Brain();
            rt.OnStepCallback = (entity, gameState) => UseBrain(brain, entity, gameState);

            PublishArray(rt);
        }

        private void UseBrain(Brain brain, GameEntity entity, GameState gameState)
        {
            if (entity.LastDamageTaken != null)
            {
                MovementStruct ms = new MovementStruct();
                ms.FollowId = entity.LastDamageTaken.AttackerId;
                var updateMovementEvent = new UpdateMovementEvent(entity.Identifier.Id, ms);
                PublishArray(updateMovementEvent);
            }
#if false
            if (c++ % 100 == 0)
            {
                var ms = new MovementStruct();
                ms.Direction = (float)(RNG.BetweenZeroAndOne() * Math.PI * 2);
                PublishArray(new UpdateMovementEvent(entity.Identifier.Id, ms));
            }
#endif
        }

        public void SpawnWorld()
        {
            var house = GameState.EntityGenerator.GenerateHouse(5, 5).Select(e => new CreateEntityEvent(e));

            Publish(house);
        }

    }
}
