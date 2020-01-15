namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using whateverthefuck.src.model.entities;
    using System.Linq;

    public enum AbilityType
    {
        Fireball,
        Fireburst,

        Sanic,

        Bite,
    }

    public abstract class Ability
    {
        protected const float MeleeRange = 0.2f;

        public Ability(AbilityType abilityType)
        {
            this.AbilityType = abilityType;
        }

        public int CastTime { get; protected set; }

        public int BaseCooldown { get; protected set; }

        public int CurrentCooldown { get; set; }

        public AbilityType AbilityType { get; protected set; }

        public float Range { get; protected set; }

        public bool CreateProjectile { get; protected set; }

        public float CooldownPercentage => this.BaseCooldown == 0 ? 0 : (float)this.CurrentCooldown / this.BaseCooldown;

        public abstract IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState);

        /// <summary>
        /// Used when a Character finishes the casting time of an Ability and the effect of the spell is to take place.
        /// </summary>
        /// <param name="caster">The GameEntity which cast the ability.</param>
        /// <returns>The event which creates the Projectile if there should be one, null otherwise.</returns>
        public CreateEntityEvent Cast(GameEntity caster)
        {
            CreationArguments ca = new ProjectileCreationArguments(caster, this.AbilityType);
            return new CreateEntityEvent(EntityIdentifier.RandomReserved(), EntityType.Projectile, caster.Info.Center.X, caster.Info.Center.Y, 1, 1, ca);
        }

        public abstract bool CanTarget(GameEntity caster, GameEntity target, GameState gameState);
    }

    public class Fireball : Ability
    {
        public Fireball()
            : base(AbilityType.Fireball)
        {
            this.CastTime = 200;
            this.BaseCooldown = 0;
            this.Range = 1.5f;
            this.CreateProjectile = true;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, 15),
                new ApplyStatusEvent(target, new BurnStatus(caster.Info.Identifier, 10)),
            };
        }

        public override bool CanTarget(GameEntity caster, GameEntity target, GameState gameState)
        {
            return
                target != caster &&
                target.Info.State == GameEntityState.Alive &&
                target is Character;
        }
    }

    public class Bite : Ability
    {
        public Bite()
            : base(AbilityType.Bite)
        {
            this.CastTime = 0;
            this.BaseCooldown = 0;
            this.Range = MeleeRange;
            this.CreateProjectile = false;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, 1),
            };
        }

        public override bool CanTarget(GameEntity caster, GameEntity target, GameState gameState)
        {
            return
                target != caster &&
                target.Info.State == GameEntityState.Alive &&
                target is Character;
        }
    }

    public class Sanic : Ability
    {
        public Sanic()
            : base(AbilityType.Sanic)
        {
            this.CastTime = 0;
            this.BaseCooldown = 1000;
            this.Range = 0;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new ApplyStatusEvent(caster, new SanicStatus(caster.Info.Identifier, 300, 100)),
                new DealDamageEvent(caster, caster, 10),
            };
        }

        public override bool CanTarget(GameEntity caster, GameEntity target, GameState gameState)
        {
            return true;
        }
    }

    public class Fireburst : Ability
    {
        public Fireburst()
               : base(AbilityType.Fireburst)
        {
            this.CastTime = 0;
            this.BaseCooldown = 500;
            this.Range = 1.5f;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, 15),
                new ApplyStatusEvent(target, new SlowStatus(caster.Info.Identifier, 800, 80)),
                new ApplyStatusEvent(target, new VulnerableStatus(caster.Info.Identifier, 800, 15)),
            };
        }

        public override bool CanTarget(GameEntity caster, GameEntity target, GameState gameState)
        {
            return
                target != caster &&
                target.Info.State == GameEntityState.Alive &&
                target is Character;
        }
    }

    public class CastingInfo
    {
        public CastingInfo(Ability castingAbility, GameEntity target)
        {
            this.CastingAbility = castingAbility;
            this.Target = target.Info.Identifier;
            this.MaxTicks = castingAbility.CastTime;
        }

        public bool DoneCasting => this.ElapsedTicks >= this.MaxTicks;

        public float PercentageDone => (float)this.ElapsedTicks / this.MaxTicks;

        public Ability CastingAbility { get; }

        public EntityIdentifier Target { get; }

        private int ElapsedTicks { get; set; }

        private int MaxTicks { get; }

        public void Step()
        {
            this.ElapsedTicks++;
        }
    }
}
