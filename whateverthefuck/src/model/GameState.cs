﻿namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.network.messages;
    using whateverthefuck.src.util;

    public class GameState
    {
        private List<GameEntity> removeUs = new List<GameEntity>();
        private List<GameEntity> addUs = new List<GameEntity>();
        private object lockSafeList = new object();

        private IdentifierGenerator idGenerator = new IdentifierGenerator();

        public GameState()
        {
            this.EntityGenerator = new EntityGenerator(this.idGenerator);
        }

        public IEnumerable<GameEntity> AllEntities => this.EntityListSafe;

        public EntityGenerator EntityGenerator { get; private set; }

        public int StepCounter { get; set; }

        private List<GameEntity> EntityList { get; } = new List<GameEntity>();

        private List<GameEntity> EntityListSafe { get; set; } = new List<GameEntity>();

        public void UpdateLists()
        {
            lock (this.lockSafeList)
            {
                foreach (var removeMe in this.removeUs)
                {
                    this.EntityList.Remove(removeMe);
                }

                this.EntityList.AddRange(this.addUs);

                this.removeUs.Clear();
                this.addUs.Clear();

                this.EntityListSafe = new List<GameEntity>(this.EntityList);
            }
        }

        public void Step()
        {
            this.UpdateLists();

            foreach (var entity in this.AllEntities)
            {
                entity.Step(this);
            }

            this.HandleCollisions();
            this.StepCounter++;
        }

        public GameEntity GetEntityById(int id)
        {
            return this.EntityList.Find(e => e.Identifier.Id == id);
        }

        public void HandleGameEvents(params GameEvent[] es)
        {
            this.HandleGameEvents((IEnumerable<GameEvent>)es);
        }

        public void HandleGameEvents(IEnumerable<GameEvent> es)
        {
            foreach (var e in es)
            {
                this.HandleGameEvent(e);
            }
        }

        public SyncMessageBody GenerateSyncRecord()
        {
            var rt = new SyncMessageBody(this.StepCounter, this.HashMe());

            if (false)
            {
                Logging.Log(string.Format("Tick '{0}' Hash '{1}'", rt.Tick, rt.Hash.ToString("X8")));
            }

            return rt;
        }

        public int HashMe()
        {
            int hash = 0;

            foreach (var e in this.AllEntities)
            {
                if (e.Identifier.Id < 0)
                {
                    continue;
                }

                hash ^= BitConverter.ToInt32(BitConverter.GetBytes(e.Location.X), 0);
                hash ^= BitConverter.ToInt32(BitConverter.GetBytes(e.Location.Y), 0);
                hash ^= (int)e.EntityType;
            }

            return hash;
        }

        public IEnumerable<GameEntity> Intersects(GameEntity e)
        {
            List<GameEntity> rt = new List<GameEntity>();

            foreach (var entity in this.EntityListSafe)
            {
                if (e != entity && this.DetectCollisions(e, entity).HasValue)
                {
                    rt.Add(entity);
                }
            }

            return rt;
        }

        private void HandleCollisions()
        {
            var collisions = this.DetectCollisions();
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

                if (collision.Direction == CollisionRecord.CollisionDirection.Left)
                {
                    var ishare = collision.EntityI.MovementCache.X / (collision.EntityI.MovementCache.X + collision.EntityJ.MovementCache.X);
                    var jshare = 1 - ishare;
                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.Location.X -= ishare * collision.Overlap;
                        collision.EntityJ.Location.X += jshare * collision.Overlap;
                    }
                }
                else if (collision.Direction == CollisionRecord.CollisionDirection.Right)
                {
                    var ishare = collision.EntityI.MovementCache.X / (collision.EntityI.MovementCache.X + collision.EntityJ.MovementCache.X);
                    var jshare = 1 - ishare;

                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.Location.X += ishare * collision.Overlap;
                        collision.EntityJ.Location.X -= jshare * collision.Overlap;
                    }
                }
                else if (collision.Direction == CollisionRecord.CollisionDirection.Top)
                {
                    var ishare = collision.EntityI.MovementCache.Y / (collision.EntityI.MovementCache.Y + collision.EntityJ.MovementCache.Y);
                    var jshare = 1 - ishare;

                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.Location.Y += ishare * collision.Overlap;
                        collision.EntityJ.Location.Y -= jshare * collision.Overlap;
                    }
                }
                else if (collision.Direction == CollisionRecord.CollisionDirection.Bottom)
                {
                    var ishare = collision.EntityI.MovementCache.Y / (collision.EntityI.MovementCache.Y + collision.EntityJ.MovementCache.Y);
                    var jshare = 1 - ishare;

                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.Location.Y -= ishare * collision.Overlap;
                        collision.EntityJ.Location.Y += jshare * collision.Overlap;
                    }
                }
            }
        }

        private bool HandleGameEvent(GameEvent e)
        {
            if (e is CreateEntityEvent)
            {
                CreateEntityEvent cee = (CreateEntityEvent)e;
                var entity = this.EntityGenerator.GenerateEntity(cee);
                entity.OnDeath += cee.OnDeathCallback;
                entity.OnStep += cee.OnStepCallback;

                // it has to be like this
                cee.OnCreationCallback?.Invoke(entity);

                this.AddEntities(entity);
            }
            else if (e is DestroyEntityEvent)
            {
                DestroyEntityEvent dee = (DestroyEntityEvent)e;
                var entity = this.GetEntityById(dee.Id);
                this.RemoveEntity(entity);
            }
            else if (e is UpdateMovementEvent)
            {
                UpdateMovementEvent uce = (UpdateMovementEvent)e;
                var entity = this.GetEntityById(uce.Id);

                if (entity == null)
                {
                    Logging.Log("Dubious UpdateMovementEvent");
                    return false;
                }

                entity.Movements = uce.Movements;
            }
            else if (e is UseAbilityEvent)
            {
                UseAbilityEvent uae = (UseAbilityEvent)e;
                var caster = this.GetEntityById(uae.Id);
                var castee = this.GetEntityById(uae.TargetId);

                if (caster == null || castee == null)
                {
                    Logging.Log("Dubious UseAbilityEvent");
                    return false;
                }

                var ca = new ProjectileArgs(caster);
                var p = this.EntityGenerator.GenerateEntity(EntityType.Projectile, EntityIdentifier.RandomReserved(), ca);
                p.Location = caster.Center;
                p.Movements.FollowId = castee.Identifier.Id;
                this.AddEntities(p);
            }
            else if (e is DealDamageEvent)
            {
                DealDamageEvent damage = (DealDamageEvent)e;

                var attacker = this.GetEntityById(damage.AttackerId);
                var defender = this.GetEntityById(damage.DefenderId);

                defender.CurrentHealth -= damage.Damage;
                defender.LastDamageTaken = damage;
            }
            else
            {
                throw new NotImplementedException();
            }

            return true;
        }

        private void AddEntities(params GameEntity[] entities)
        {
            lock (this.lockSafeList)
            {
                this.EntityList.AddRange(entities);
            }
        }

        private void RemoveEntity(GameEntity entity)
        {
            lock (this.lockSafeList)
            {
                this.EntityList.Remove(entity);
            }
        }

        private void RemoveEntity(EntityIdentifier id)
        {
            this.RemoveEntity(this.GetEntityById(id.Id));
        }

        private CollisionRecord? DetectCollisions(GameEntity entityI, GameEntity entityJ)
        {
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

                CollisionRecord.CollisionDirection direction = CollisionRecord.CollisionDirection.None;

                if (d == x1) { direction = CollisionRecord.CollisionDirection.Left; }
                if (d == x2) { direction = CollisionRecord.CollisionDirection.Right; }
                if (d == y1) { direction = CollisionRecord.CollisionDirection.Bottom; }
                if (d == y2) { direction = CollisionRecord.CollisionDirection.Top; }

                return new CollisionRecord(entityI, entityJ, direction, d);
            }
            else
            {
                return null;
            }
        }

        private List<CollisionRecord> DetectCollisions()
        {
            var collisions = new List<CollisionRecord>();

            for (int i = 0; i < this.EntityList.Count; i++)
            {
                var entityI = this.EntityList[i];
                if (!entityI.Collidable) { continue; }

                for (int j = i + 1; j < this.EntityList.Count; j++)
                {
                    var entityJ = this.EntityList[j];

                    if (!entityJ.Collidable || (!entityI.Movable && !entityJ.Movable)) { continue; }

                    var collision = this.DetectCollisions(entityI, entityJ);
                    if (collision.HasValue)
                    {
                        collisions.Add(collision.Value);
                    }
                }
            }

            return collisions;
        }

        private struct CollisionRecord
        {
            public CollisionRecord(GameEntity entityI, GameEntity entityJ, CollisionDirection direction, float overlap)
            {
                this.EntityI = entityI;
                this.EntityJ = entityJ;
                this.Direction = direction;
                this.Overlap = overlap;
            }

            public enum CollisionDirection
            {
                None,
                Left,
                Right,
                Top,
                Bottom,
            }

            public GameEntity EntityI { get; }

            public GameEntity EntityJ { get; }

            public CollisionDirection Direction { get; }

            public float Overlap { get; }

            public bool IsCollision => this.Direction != CollisionDirection.None;
        }
    }
}
