namespace whateverthefuck.src.model.entities
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class Projectile : GameEntity
    {
        public Projectile(EntityIdentifier id, CreationArgs args)
            : base(id, EntityType.Projectile, args)
        {
            if (args.Value != 0)
            {
                this.Controller = new ProjectileArgs(args).ControllerId;
            }

            ProjectileArgs pa = new ProjectileArgs(args);

            this.Size = new GameCoordinate(0.1f, 0.1f);
            this.DrawColor = Color.Black;
            this.MoveSpeed = 0.02f;
            this.Collidable = false;

            this.Sprite = pa.GetSprite();
        }

        public IEnumerable<GameEvent> ResolveEvents { get; set; }

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
            return this.ResolveEvents.Append(new DestroyEntityEvent(this));
        }

        private IEnumerable<GameEvent> Fizzle()
        {
            return new GameEvent[] { new DestroyEntityEvent(this) };
        }
    }

    public class ProjectileArgs : CreationArgs
    {
        public ProjectileArgs(GameEntity controller, AbilityType abilityType)
            : base(0)
        {
            this.ControllerId = controller.Identifier.Id;
            this.AbilityType = abilityType;
        }

        public ProjectileArgs(CreationArgs a)
            : base(a.Value)
        {
        }

        public AbilityType AbilityType
        {
            get { return (AbilityType)this.FirstInt; }
            set { this.FirstInt = (int)value; }
        }

        public int ControllerId
        {
            get { return this.SecondInt; }
            set { this.SecondInt = value; }
        }

        public Sprite GetSprite()
        {
            return Sprite.GetSprite(this.AbilityType);
        }
    }
}
