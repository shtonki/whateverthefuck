namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using whateverthefuck.src.model.entities;
    using System.Linq;
    using whateverthefuck.src.view.guicomponents;
    using System.Text;

    public enum AbilityType
    {
        Fireball,
        Fireburst,
        Sanic,
        Bite,
        Mend,
    }

    public enum NPCBrainAbilityTags
    {
        Offensive,
        Defensive,
    }

    public abstract class Ability : ToolTipper
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

        protected string ToolTip { get; set; }

        public bool CreateProjectile { get; protected set; }

        public float CooldownPercentage => this.BaseCooldown == 0 ? 0 : (float)this.CurrentCooldown / this.BaseCooldown;

        private bool[] NPCBrainTags { get; } = new bool[Enum.GetValues(typeof(NPCBrainAbilityTags)).Length];

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

        public bool HasTag(NPCBrainAbilityTags tag)
        {
            return this.NPCBrainTags[(int)tag];
        }

        public string GetToolTip()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(AbilityType.ToString());
            sb.Append(Environment.NewLine);

            if (ToolTip == null) { sb.Append("Tooltip missing..."); }
            else { sb.Append(ToolTip); }
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        protected void AddTag(NPCBrainAbilityTags tag)
        {
            this.NPCBrainTags[(int)tag] = true;
        }

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

            ToolTip = String.Format("Deals big damage and applies Burn");
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            int initialDamage = caster.Status.ReadCurrentStats.Intelligence;
            int burnStacks = caster.Status.ReadCurrentStats.Intelligence;

            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, initialDamage),
                new ApplyStatusEvent(target, new BurningStatus(caster.Info.Identifier, burnStacks)),
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
            int initialDamage = (int)(caster.Status.ReadCurrentStats.Intelligence * 992.5f);
            int slowPercentage = 80;
            int vulnerableStacks = 15;

            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, initialDamage),
                new ApplyStatusEvent(target, new SlowedStatus(caster.Info.Identifier, 800, slowPercentage)),
                new ApplyStatusEvent(target, new VulnerableStatus(caster.Info.Identifier, 800, vulnerableStacks)),
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

    public class Mend : Ability
    {
        public Mend()
            : base(AbilityType.Mend)
        {
            this.CastTime = 200;
            this.BaseCooldown = 0;
            this.Range = 1.5f;

            this.AddTag(NPCBrainAbilityTags.Defensive);
        }

        public override bool CanTarget(GameEntity caster, GameEntity target, GameState gameState)
        {
            return target is Character;
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[] { new HealEvent(caster, target, 20) };
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

            this.AddTag(NPCBrainAbilityTags.Offensive);
        }

        public override IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target, GameState gameState)
        {
            return new GameEvent[]
            {
                new DealDamageEvent(caster, target, caster.Status.ReadCurrentStats.Strength),
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
}
