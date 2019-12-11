using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
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
            var rt = new CreateEntityEvent(GameState.EntityGenerator.GenerateEntity(EntityType.PlayerCharacter));
            PublishArray(rt);
            return rt;
        }

        public void SpawnMob()
        {
            var rt = new CreateEntityEvent(GameState.EntityGenerator.GenerateEntity(EntityType.PlayerCharacter));
            rt.OnDeathCallback = (e) => SpawnLoot((GameCoordinate)e.Location);
            PublishArray(rt);
        }

        private void SpawnLoot(GameCoordinate location)
        {
            var loot = GameState.EntityGenerator.GenerateEntity(EntityType.Loot);
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
