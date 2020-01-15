namespace whateverthefuck.src.model.entities
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class Projectile : GameEntity
    {
        public Projectile(EntityIdentifier id, CreationArguments args)
            : this(id, args as ProjectileCreationArguments)
        {
        }

        public Projectile(EntityIdentifier id, ProjectileCreationArguments args)
            : base(id, EntityType.Projectile, args)
        {
            this.Controller = args.ControllerId;

            this.Sprite = new Sprite(args.GetSpriteID());

            this.Info.Size = new GameCoordinate(0.1f, 0.1f);
            this.DrawColor = Color.Black;
            this.Info.Collidable = false;
            this.Info.Movable = true;

            this.Status.BaseStats.MoveSpeed = 0.02f;
        }

        public IEnumerable<GameEvent> ResolveEvents { get; set; }

        private EntityIdentifier Controller { get; }

        private float AsplodeCutoff => this.Status.ReadCurrentStats.MoveSpeed * 2;

        public override void Step(GameState gameState)
        {
            base.Step(gameState);

            if (this.Movements.IsFollowing)
            {
                var followed = gameState.GetEntityById(this.Movements.FollowId);

                if (followed == null)
                {
                    gameState.HandleGameEvents(this.Fizzle());
                }
                else if (this.DistanceTo(followed.Info.Center) < this.AsplodeCutoff)
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

    public class ProjectileCreationArguments : CreationArguments
    {
        public ProjectileCreationArguments(GameEntity controller, AbilityType abilityType)
        {
            this.ControllerId = controller.Info.Identifier;
            this.AbilityType = abilityType;
        }

        public AbilityType AbilityType { get; private set; }

        public EntityIdentifier ControllerId { get; private set; }

        public SpriteID GetSpriteID()
        {
            switch (this.AbilityType)
            {
                case AbilityType.Fireball:
                {
                    return SpriteID.projectile_Fireball;
                }

                default: return SpriteID.testSprite1;
            }

        }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.AbilityType);
            encoder.Encode(this.ControllerId.Id);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.AbilityType = (AbilityType)decoder.DecodeInt();
            this.ControllerId = new EntityIdentifier(decoder.DecodeInt());
        }
    }
}
