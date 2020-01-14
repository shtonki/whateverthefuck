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

        public TargetingRule TargetingRule { get; protected set; }

        public float CooldownPercentage => this.BaseCooldown == 0 ? 0 : (float)this.CurrentCooldown / this.BaseCooldown;

        public abstract IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState);

        /// <summary>
        /// Used when a Character finishes the casting time of an Ability and the effect of the spell is to take place.
        /// </summary>
        /// <param name="caster">The GameEntity which cast the ability.</param>
        /// <returns>The event which creates the Projectile if there should be one, null otherwise.</returns>
        public CreateEntityEvent Cast(GameEntity caster)
        {
            CreationArgs ca = new ProjectileArgs(caster, this.AbilityType);
            return new CreateEntityEvent(EntityIdentifier.RandomReserved(), EntityType.Projectile, caster.Center.X, caster.Center.Y, 1, 1, ca);
        }
    }

    public class Fireball : Ability
    {
        public Fireball()
            : base(AbilityType.Fireball)
        {
            this.CastTime = 50;
            this.BaseCooldown = 0;
            this.Range = 1.5f;
            this.TargetingRule = TargetingRule.IsAliveEnemyCharacter;
            this.CreateProjectile = true;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, 15),
            };
        }
    }

    public class Bite : Ability
    {
        public Bite()
            : base(AbilityType.Bite)
        {
            this.CastTime = 10;
            this.BaseCooldown = 0;
            this.Range = MeleeRange;
            this.TargetingRule = TargetingRule.IsAliveEnemyCharacter;
            this.CreateProjectile = false;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, 20),
            };
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
            this.TargetingRule = TargetingRule.Any;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new ApplyStatusEvent(caster, new SanicStatus(300, 1)),
            };
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
            this.TargetingRule = TargetingRule.IsAliveEnemyCharacter;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, 15),
            };
        }
    }

    public class TargetingRule
    {
        private TargetingRule(Func<GameEntity, GameEntity, GameState, bool> rule)
        {
            this.Rule = rule;
        }

        public static TargetingRule IsNotDead { get; } = new TargetingRule((caster, target, state) => target.State != GameEntityState.Dead);

        public static TargetingRule IsNotSelf { get; } = new TargetingRule((caster, target, state) => target != caster);

        public static TargetingRule IsSelf { get; } = new TargetingRule((caster, target, state) => target == caster);

        public static TargetingRule IsCharacter { get; } = new TargetingRule((caster, target, state) => target is Character);

        public static TargetingRule Any { get; } = new TargetingRule((caster, target, state) => true);

        public static TargetingRule IsAliveEnemyCharacter { get; } = ConstructRuleWithAnd(TargetingRule.IsNotDead, TargetingRule.IsNotSelf, TargetingRule.IsCharacter);

        private Func<GameEntity, GameEntity, GameState, bool> Rule { get; }

        public static TargetingRule ConstructRuleWithAnd(params TargetingRule[] rules)
        {
            return new TargetingRule((caster, target, state) => rules.All(r => r.ApplyRule(caster, target, state)));
        }

        public static TargetingRule ConstructRuleWithOr(params TargetingRule[] rules)
        {
            return new TargetingRule((caster, target, state) => rules.Any(r => r.ApplyRule(caster, target, state)));
        }

        public bool ApplyRule(GameEntity caster, GameEntity target, GameState state)
        {
            return this.Rule(caster, target, state);
        }
    }

    public class CastingInfo
    {
        public CastingInfo(Ability castingAbility, GameEntity target)
        {
            this.CastingAbility = castingAbility;
            this.Target = target;
            this.MaxTicks = castingAbility.CastTime;
        }

        public bool DoneCasting => this.ElapsedTicks >= this.MaxTicks;

        public float PercentageDone => (float)this.ElapsedTicks / this.MaxTicks;

        public Ability CastingAbility { get; }

        public GameEntity Target { get; }

        private int ElapsedTicks { get; set; }

        private int MaxTicks { get; }

        public void Step()
        {
            this.ElapsedTicks++;
        }
    }
}
