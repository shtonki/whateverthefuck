using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using whateverthefuck.src.control;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    public class GameState
    {
        private List<GameEntity> EntityList { get; } = new List<GameEntity>();
        private List<GameEntity> EntityListSafe { get; set; } = new List<GameEntity>();

        private List<GameEntity> RemoveUs = new List<GameEntity>();
        private List<GameEntity> AddUs = new List<GameEntity>();
        private object LockSafeList = new object();


        public IEnumerable<GameEntity> AllEntities => EntityListSafe;

        public void UpdateLists()
        {
            lock (LockSafeList)
            {
                foreach (var removeMe in RemoveUs)
                {
                    EntityList.Remove(removeMe);
                }

                EntityList.AddRange(AddUs);

                RemoveUs.Clear();
                AddUs.Clear();

                EntityListSafe = new List<GameEntity>(EntityList);
            }
        }

        private IdentifierGenerator IdGenerator = new IdentifierGenerator();

        public EntityGenerator EntityGenerator { get; private set; }

        public GameState()
        {
            EntityGenerator = new EntityGenerator(IdGenerator);
        }

        public void AddEntity(GameEntity entity)
        {
            lock (LockSafeList)
            {
                EntityList.Add(entity);
            }

            Logging.Log("Added ID: " + entity.Identifier.Id);
        }

        public void RemoveEntity(GameEntity entity)
        {
            lock (LockSafeList)
            {
                EntityList.Remove(entity);
            }
        }

        public void RemoveEntity(EntityIdentifier id)
        {
            RemoveEntity(GetEntityById(id.Id));
        }

        public GameEntity GetEntityById(int id)
        {
            return EntityList.Find(e => e.Identifier.Id == id);
        }

        private void HandleCollisions()
        {
            var collisions = DetectCollisions();
            collisions.Sort((r1, r2) => r1.Overlap.CompareTo(-r2.Overlap));

            foreach (var collision in collisions)
            {

                if (collision.EntityI?.MovementCache?.X == 0 &&
                    collision.EntityI?.MovementCache?.Y == 0 &&
                    collision.EntityJ?.MovementCache?.X == 0 &&
                    collision.EntityJ?.MovementCache?.Y == 0)
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

        public void Step()
        {
            foreach (var entity in AllEntities)
            {
                entity.Step();
            }

            HandleCollisions();

            UpdateLists();
        }

        private List<CollisionRecord> DetectCollisions()
        {
            var collisions = new List<CollisionRecord>();

            for (int i = 0; i < EntityList.Count; i++)
            {
                var entityI = EntityList[i];
                if (!entityI.Collidable) { continue; }

                for (int j = i+1; j < EntityList.Count; j++)
                {
                    var entityJ = EntityList[j];
                    if (!entityJ.Collidable) { continue; }

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
    }
}
