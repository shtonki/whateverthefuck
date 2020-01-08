namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.network.messages;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

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

        public EntityDrawingInfo EntityDrawingInfo { get; private set; }

        public EntityGenerator EntityGenerator { get; private set; }

        public Camera CurrentCamera { get; set; }

        public int StepCounter { get; set; }

        private List<GameEntity> EntityList { get; } = new List<GameEntity>();

        private List<GameEntity> EntityListSafe { get; set; } = new List<GameEntity>();

        public void UpdateLists()
        {
            lock (this.lockSafeList)
            {
                foreach (var entity in this.AllEntities)
                {
                    if (entity.Destroy)
                    {
                        this.removeUs.Add(entity);
                    }
                }

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

            this.UpdateEntityDrawingInfo();
        }

        private void UpdateEntityDrawingInfo()
        {
            if (this.CurrentCamera == null)
            {
                return;
            }

            var visibleEntities = this.AllEntities.Select(e => new EntityDrawable(e)).ToList();
            var cameraClone = new StaticCamera(new GameCoordinate(this.CurrentCamera.Location.X, this.CurrentCamera.Location.Y));

            this.EntityDrawingInfo = new EntityDrawingInfo(cameraClone, visibleEntities);
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
#pragma warning disable 162
            {
                Logging.Log(string.Format("Tick '{0}' Hash '{1}'", rt.Tick, rt.Hash.ToString("X8")));
            }
#pragma warning restore 162

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

                hash ^= BitConverter.ToInt32(BitConverter.GetBytes(e.GameLocation.X), 0);
                hash ^= BitConverter.ToInt32(BitConverter.GetBytes(e.GameLocation.Y), 0);
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
                        collision.EntityI.GameLocation.X -= ishare * collision.Overlap;
                        collision.EntityJ.GameLocation.X += jshare * collision.Overlap;
                    }
                }
                else if (collision.Direction == CollisionRecord.CollisionDirection.Right)
                {
                    var ishare = collision.EntityI.MovementCache.X / (collision.EntityI.MovementCache.X + collision.EntityJ.MovementCache.X);
                    var jshare = 1 - ishare;

                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.GameLocation.X += ishare * collision.Overlap;
                        collision.EntityJ.GameLocation.X -= jshare * collision.Overlap;
                    }
                }
                else if (collision.Direction == CollisionRecord.CollisionDirection.Top)
                {
                    var ishare = collision.EntityI.MovementCache.Y / (collision.EntityI.MovementCache.Y + collision.EntityJ.MovementCache.Y);
                    var jshare = 1 - ishare;

                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.GameLocation.Y += ishare * collision.Overlap;
                        collision.EntityJ.GameLocation.Y -= jshare * collision.Overlap;
                    }
                }
                else if (collision.Direction == CollisionRecord.CollisionDirection.Bottom)
                {
                    var ishare = collision.EntityI.MovementCache.Y / (collision.EntityI.MovementCache.Y + collision.EntityJ.MovementCache.Y);
                    var jshare = 1 - ishare;

                    if (!float.IsNaN(ishare))
                    {
                        collision.EntityI.GameLocation.Y -= ishare * collision.Overlap;
                        collision.EntityJ.GameLocation.Y += jshare * collision.Overlap;
                    }
                }
            }
        }

        private void HandleGameEvent(GameEvent e)
        {
            if (e is CreateEntityEvent cee)
            {
                this.HandleEvent(cee);
            }
            else if (e is DestroyEntityEvent dee)
            {
                this.HandleEvent(dee);
            }
            else if (e is UpdateMovementEvent uce)
            {
                this.HandleEvent(uce);
            }
            else if (e is BeginCastAbilityEvent uae)
            {
                this.HandleEvent(uae);
            }
            else if (e is DealDamageEvent dde)
            {
                this.HandleEvent(dde);
            }
            else if (e is EndCastAbility eca)
            {
                this.HandleEvent(eca);
            }
            else if (e is ApplyStatusEvent ase)
            {
                this.HandleEvent(ase);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void HandleEvent(ApplyStatusEvent applyStatusEvent)
        {
            applyStatusEvent.Entity.ApplyStatus(applyStatusEvent.AppliedStatus);
        }

        private void HandleEvent(BeginCastAbilityEvent beginCastAbility)
        {
            var caster = this.GetEntityById(beginCastAbility.CasterId);
            var target = this.GetEntityById(beginCastAbility.TargetId);
            var ability = caster.Ability(beginCastAbility.AbilityType);

            caster.CastAbility(ability, target);
        }

        private void HandleEvent(EndCastAbility endCastAbilityEvent)
        {
            var caster = this.GetEntityById(endCastAbilityEvent.CasterId);
            var castee = this.GetEntityById(endCastAbilityEvent.TargetId);

            if (caster == null || castee == null)
            {
                Logging.Log("Dubious UseAbilityEvent");
                return;
            }

            Ability ability = caster.Ability(endCastAbilityEvent.AbilityType);

            if (ability.CreateProjectile)
            {

                CreateEntityEvent projectileCreationEvent = ability.Cast(caster);
                var projectile = (Projectile)this.EntityGenerator.GenerateEntity(projectileCreationEvent);
                projectile.Center = caster.Center;
                projectile.Movements.FollowId = castee.Identifier.Id;
                projectile.ResolveEvents = ability.Resolve(caster, castee);
                this.AddEntities(projectile);
            }
            else
            {
                this.HandleGameEvents(ability.Resolve(caster, castee));
            }
        }

        private void HandleEvent(UpdateMovementEvent updateMovementEvent)
        {
            var entity = this.GetEntityById(updateMovementEvent.Id);

            if (entity == null)
            {
                Logging.Log("Dubious UpdateMovementEvent");
                return;
            }

            entity.Movements = updateMovementEvent.Movements;
        }

        private void HandleEvent(DealDamageEvent dealDamageEvent)
        {
            var defender = this.GetEntityById(dealDamageEvent.DefenderId);

            defender.CurrentHealth -= dealDamageEvent.Damage;
            if (defender.CurrentHealth < 0)
            {
                defender.CurrentHealth = 0;
            }
            defender.LastDamageTaken = dealDamageEvent;

            if (Program.GameStateManager.Hero != null)
            {
                if (dealDamageEvent.AttackerId == Program.GameStateManager.Hero.Identifier.Id)
                {
                    GUI.AddDamageText(defender.Center, dealDamageEvent.Damage.ToString(), Color.Orange);
                }
                else if (dealDamageEvent.DefenderId == Program.GameStateManager.Hero.Identifier.Id)
                {
                    GUI.AddDamageText(defender.Center, dealDamageEvent.Damage.ToString(), Color.DarkRed);
                }
            }
        }

        private void HandleEvent(CreateEntityEvent createEntityEvent)
        {
            var entity = this.EntityGenerator.GenerateEntity(createEntityEvent);
            entity.OnDeath += createEntityEvent.OnDeathCallback;
            entity.OnStep += createEntityEvent.OnStepCallback;

            // it has to be like this
            createEntityEvent.OnCreationCallback?.Invoke(entity);

            this.AddEntities(entity);
        }

        private void HandleEvent(DestroyEntityEvent destroyEntityEvent)
        {
            var entity = this.GetEntityById(destroyEntityEvent.Id);
            this.RemoveEntity(entity);
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


    public class EntityDrawingInfo
    {
        public EntityDrawingInfo(Camera camera, List<EntityDrawable> entityDrawables)
        {
            this.Camera = camera;
            this.EntityDrawables = entityDrawables;
        }

        public Camera Camera { get; }

        public Guid id { get; }

        public List<EntityDrawable> EntityDrawables { get; }
    }

}
