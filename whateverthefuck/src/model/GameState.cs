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

            var npc1 = new Character();
            npc1.Location.X = 0.5f;
            npc1.Location.Y = 0.5f;
            npc1.Size.Y = 0.55f;
            AllEntities.Add(npc1);

            TickTimer = new Timer(Step, null, 0, 10);
        }

        private void Step(object state)
        {
            foreach (var entity in AllEntities)
            {
                entity.Step();
            }

            var collisions = DetectCollisions();

            foreach (var collision in collisions)
            {
                collision.EntityI.UndoLastMovement();
                collision.EntityJ.UndoLastMovement();
            }
        }

        private List<CollisionRecord> DetectCollisions()
        {
            var collisions = new List<CollisionRecord>();

            for (int i = 0; i < AllEntities.Count; i++)
            {
                var entityI = AllEntities[i];

                for (int j = i+1; j < AllEntities.Count; j++)
                {
                    var entityJ = AllEntities[j];

                    var check1 = entityI.Left < entityJ.Right;
                    var check2 = entityI.Right > entityJ.Left;
                    var check3 = entityI.Top > entityJ.Bottom;
                    var check4 = entityI.Bottom < entityJ.Top;

                    var collision = check1 && check2 && check3 && check4;

                    if (collision)
                    {
                        var x1 = entityI.Right - entityJ.Left;
                        var x2 = entityJ.Right - entityI.Left;
                        var y1 = entityI.Top - entityJ.Bottom;
                        var y2 = entityJ.Top - entityI.Bottom;

                        var d = Math.Min(Math.Min(Math.Min(x1, x2), y1), y2);

                        CollisionDirection direction = CollisionDirection.None;

                        if (d == x1) { direction = CollisionDirection.Left; }
                        if (d == x2) { direction = CollisionDirection.Right; }
                        if (d == y1) { direction = CollisionDirection.Bottom; }
                        if (d == y2) { direction = CollisionDirection.Top; }

                        collisions.Add(new CollisionRecord(entityI, entityJ, direction));
                    }
                }
            }

            return collisions;
        }

        enum CollisionDirection
        {
            None, Left, Right, Top, Bottom,
        }

        struct CollisionRecord
        {
            public GameEntity EntityI { get; }
            public GameEntity EntityJ { get; }
            public CollisionDirection Direction { get; }

            public CollisionRecord(GameEntity entityI, GameEntity entityJ, CollisionDirection direction)
            {
                EntityI = entityI;
                EntityJ = entityJ;
                Direction = direction;
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
