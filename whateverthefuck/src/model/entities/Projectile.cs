namespace whateverthefuck.src.model.entities
{
    using System.Collections.Generic;
    using System.Drawing;

    public class Projectile : GameEntity
    {
        public Projectile(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Projectile, args)
        {
            if (args.Value != 0)
            {
                this.Controller = new ProjectileArgs(args).ControllerId;
            }

            this.Size = new GameCoordinate(0.01f, 0.01f);
            this.DrawColor = Color.Black;
            this.MoveSpeed = 0.02f;
            this.Collidable = false;
        }

        private int Controller { get; }

        private float AsplodeCutoff => this.MoveSpeed * 2;

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

        private IEnumerable<GameEvent> Boom(GameEntity followed)
        {
            return new GameEvent[] { new DealDamageEvent(this.Controller, followed.Identifier.Id, 10), new DestroyEntityEvent(this) };
        }

        private IEnumerable<GameEvent> Fizzle()
        {
            return new GameEvent[] { new DestroyEntityEvent(this) };
        }
    }

    public class ProjectileArgs : CreationArgs
    {
        public ProjectileArgs(GameEntity c)
            : base(0)
        {
            this.ControllerId = c.Identifier.Id;
        }

        public ProjectileArgs(CreationArgs a)
            : base(a.Value)
        {
        }

        public int ControllerId
        {
            get { return this.SecondInt; }
            set { this.SecondInt = value; }
        }
    }
}
