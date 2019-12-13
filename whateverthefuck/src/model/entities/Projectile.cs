namespace whateverthefuck.src.model.entities
{
    using System.Collections.Generic;
    using System.Drawing;

    internal class Projectile : GameEntity
    {
        private float AsplodeCutoff => this.MoveSpeed * 2;

        private int controller;

        public Projectile(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Projectile, args)
        {
            if (args.Value != 0)
            {
                this.controller = new ProjectileArgs(args).ControllerId;
            }

            this.Size = new GameCoordinate(0.01f, 0.01f);
            this.DrawColor = Color.Black;
            this.MoveSpeed = 0.02f;
            this.Collidable = false;
        }

        private IEnumerable<GameEvent> Boom(GameEntity followed)
        {
            return new GameEvent[] { new DealDamageEvent(this.controller, followed.Identifier.Id, 10), new DestroyEntityEvent(this) };
        }

        private IEnumerable<GameEvent> Fizzle()
        {
            return new GameEvent[] { new DestroyEntityEvent(this) };
        }

        public override void Step(GameState gameState)
        {
            base.Step(gameState);

            if (this.Movements.IsFollowing)
            {
                var followed = gameState.GetEntityById(this.Movements.FollowId.Value);

                if (followed == null)
                {
                    gameState.HandleGameEvents(this.Fizzle());
                }
                else if (this.DistanceTo(followed.Center) < this.AsplodeCutoff)
                {
                    gameState.HandleGameEvents(this.Boom(followed));
                }
            }
        }
    }

    public class ProjectileArgs : CreationArgs
    {
        public int ControllerId
        {
            get { return this.SecondInt; }
            set { this.SecondInt = value; }
        }

        public ProjectileArgs(GameEntity c)
            : base(0)
        {
            this.ControllerId = c.Identifier.Id;
        }

        public ProjectileArgs(CreationArgs a)
            : base(a.Value)
        {
        }
    }
}
