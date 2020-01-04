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

        Bite,
    }

    public class Ability
    {
        private const float MeleeRange = 0.2f;

        public Ability(AbilityType abilityType)
        {
            this.AbilityType = abilityType;

            this.LoadBaseStats(this.AbilityType);
        }

        public int CastTime { get; private set; }

        public int BaseCooldown { get; private set; }

        public int CurrentCooldown { get; set; }

        public AbilityType AbilityType { get; private set; }

        public float Range { get; private set; }

        public bool CreateProjectile { get; private set; }

        public TargetingRule TargetingRule { get; private set; }

        public float CooldownPercentage => this.BaseCooldown == 0 ? 0 : (float)this.CurrentCooldown / this.BaseCooldown;

        public IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target)
        {
            List<GameEvent> events = new List<GameEvent>();

            switch (this.AbilityType)
            {
                case AbilityType.Fireball:
                {
                    events.Add(new DealDamageEvent(caster, target, 15));
                } break;

                case AbilityType.Fireburst:
                {
                    events.Add(new DealDamageEvent(caster, target, 420));
                } break;

                case AbilityType.Bite:
                {
                    events.Add(new DealDamageEvent(caster, target, 20));
                } break;

                default: throw new NotImplementedException();
            }

            return events;
        }

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

        private void LoadBaseStats(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Fireball:
                {
                    this.CastTime = 50;
                    this.BaseCooldown = 0;
                    this.Range = 0.5f;
                    this.TargetingRule = TargetingRule.IsAliveEnemyCharacter;
                    this.CreateProjectile = true;
                } break;

                case AbilityType.Fireburst:
                {
                    this.CastTime = 0;
                    this.BaseCooldown = 400;
                    this.Range = 0.9f;
                    this.TargetingRule = TargetingRule.IsAliveEnemyCharacter;
                    this.CreateProjectile = true;
                } break;

                case AbilityType.Bite:
                {
                    this.CastTime = 0;
                    this.BaseCooldown = 0;
                    this.Range = MeleeRange;
                    this.TargetingRule = TargetingRule.IsAliveEnemyCharacter;
                } break;

                default: throw new NotImplementedException();
            }
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
