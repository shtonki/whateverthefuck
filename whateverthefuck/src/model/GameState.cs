using System;
using System.Collections.Generic;
using System.Threading;
using whateverthefuck.src.control;

namespace whateverthefuck.src.model
{
    class GameState
    {
        public List<GameEntity> AllEntities = new List<GameEntity>();

        private Hero Hero;

        private Timer TickTimer;


        public GameState()
        {
            Hero = new Hero();
            AllEntities.Add(Hero);

            TickTimer = new Timer(Step, null, 0, 10);
        }

        private void Step(object state)
        {
            foreach (var entity in AllEntities)
            {
                entity.Step();
            }
        }

        public void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.HeroWalkUpwards:
                {
                    Hero.SetMovementUpwards(true);
                } break;

                case GameAction.HeroWalkUpwardsStop:
                {
                    Hero.SetMovementUpwards(false);
                } break;

                case GameAction.HeroWalkDownwards:
                {
                    Hero.SetMovementDownwards(true);
                } break;

                case GameAction.HeroWalkDownwardsStop:
                {
                    Hero.SetMovementDownwards(false);
                } break;

                case GameAction.HeroWalkLeftwards:
                {
                    Hero.SetMovementLeftwards(true);
                } break;

                case GameAction.HeroWalkLeftwardsStop:
                {
                    Hero.SetMovementLeftwards(false);
                } break;

                case GameAction.HeroWalkRightwards:
                {
                    Hero.SetMovementRightwards(true);
                } break;

                case GameAction.HeroWalkRightwardsStop:
                {
                    Hero.SetMovementRightwards(false);
                } break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }
    }
}
