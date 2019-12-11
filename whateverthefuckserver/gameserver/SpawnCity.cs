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
            var mob = GameState.EntityGenerator.GenerateEntity(EntityType.NPC, new CreationArgs(0));
            mob.Location = new GameCoordinate(-0.5f, -0.5f);
            var rt = new CreateEntityEvent(mob);
            rt.OnDeathCallback = (e) => SpawnLoot((GameCoordinate)e.Location);
            rt.OnStepCallback = (e) => stepme(e);
            PublishArray(rt);
        }

        int c = 0;

        private void stepme(GameEntity e)
        {
            if (c++ % 100 == 0)
            {
                var ms = new MovementStruct();
                ms.Direction = (float)(RNG.BetweenZeroAndOne() * Math.PI);
                PublishArray(new UpdateMovementEvent(e.Identifier.Id, ms));
            }
        }

        private void SpawnLoot(GameCoordinate location)
        {
            var loot = GameState.EntityGenerator.GenerateEntity(EntityType.Loot, new CreationArgs(0));
            loot.Location = new GameCoordinate(location.X, location.Y);
            var rt = new CreateEntityEvent(loot);
            PublishArray(rt);
        }

        public void SpawnWorld()
        {
            var house = GameState.EntityGenerator.GenerateHouse(5, 5).Select(e => new CreateEntityEvent(e));

            Publish(house);
        }

    }
}
