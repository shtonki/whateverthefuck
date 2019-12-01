using System;
using System.Collections.Generic;
using System.Threading;

namespace whateverthefuck.src.model
{
    class GameState
    {
        public List<GameEntity> AllEntities = new List<GameEntity>();

        private Timer TickTimer;


        public GameState()
        {
            AllEntities.Add(new GameEntity());

            TickTimer = new Timer(Step, null, 0, 10);
        }

        private void Step(object state)
        {
            foreach (var entity in AllEntities)
            {
                entity.Step();
            }
        }
    }
}
