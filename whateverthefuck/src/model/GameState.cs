using System;
using System.Collections.Generic;
using System.Threading;
using whateverthefuck.src.control;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

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
            Hero.Location = new GameCoordinate(-0.2f, -0.2f);
            AllEntities.Add(Hero);
            GUI.Camera = new FollowCamera(Hero);

            TickTimer = new Timer(Step, null, 0, 10);

            Map map = new Map(420);
            AllEntities.AddRange(map.Entities);
            AllEntities.AddRange(EntitySerializer.LoadEntitiesFromFile("testfile"));
        }

        private void Step(object state)
        {
            foreach (var entity in AllEntities)
            {
                entity.Step();
            }

            var collisions = DetectCollisions();
            collisions.Sort((r1, r2) => r1.Overlap.CompareTo(-r2.Overlap));

            foreach (var collision in collisions)
            {

                if (collision.EntityI.MovementCache.X == 0 &&
                    collision.EntityI.MovementCache.Y == 0 &&
                    collision.EntityJ.MovementCache.X == 0 &&
                    collision.EntityJ.MovementCache.Y == 0) 
                { 
                	// we have most likely managed to put one object on another at which point
                	// we let them stay there until something moves.
                	continue;
                }

                if (collision.Direction == CollisionDirection.Left)
                {
                    var ishare = collision.EntityI.MovementCache.X / (collision.EntityI.MovementCache.X + collision.EntityJ.MovementCache.X);
                    var jshare = 1 - ishare;
                    collision.EntityI.Location.X -= ishare * collision.Overlap;
                    collision.EntityJ.Location.X += jshare * collision.Overlap;
                }

                else if (collision.Direction == CollisionDirection.Right)
                {
                    var ishare = collision.EntityI.MovementCache.X / (collision.EntityI.MovementCache.X + collision.EntityJ.MovementCache.X);
                    var jshare = 1 - ishare;
                    collision.EntityI.Location.X += ishare * collision.Overlap;
                    collision.EntityJ.Location.X -= jshare * collision.Overlap;
                }

                else if (collision.Direction == CollisionDirection.Top)
                {
                    var ishare = collision.EntityI.MovementCache.Y / (collision.EntityI.MovementCache.Y + collision.EntityJ.MovementCache.Y);
                    var jshare = 1 - ishare;
                    collision.EntityI.Location.Y += ishare * collision.Overlap;
                    collision.EntityJ.Location.Y -= jshare * collision.Overlap;
                }

                else if (collision.Direction == CollisionDirection.Bottom)
                {
                    var ishare = collision.EntityI.MovementCache.Y / (collision.EntityI.MovementCache.Y + collision.EntityJ.MovementCache.Y);
                    var jshare = 1 - ishare;
                    collision.EntityI.Location.Y -= ishare * collision.Overlap;
                    collision.EntityJ.Location.Y += jshare * collision.Overlap;
                }

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

                    if (!entityI.Movable && !entityJ.Movable) { continue; }

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

                        collisions.Add(new CollisionRecord(entityI, entityJ, direction, d));
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

            public float Overlap { get; }

            public CollisionRecord(GameEntity entityI, GameEntity entityJ, CollisionDirection direction, float overlap)
            {
                EntityI = entityI;
                EntityJ = entityJ;
                Direction = direction;
                Overlap = overlap;
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

                case GameAction.CameraZoomIn:
                {
                    GUI.Camera.Zoom.ZoomIn();
                } break;

                case GameAction.CameraZoomOut:
                {
                    GUI.Camera.Zoom.ZoomOut();
                } break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }
    }
}
