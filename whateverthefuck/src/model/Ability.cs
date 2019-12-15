namespace whateverthefuck.src.model
{
    using System.Collections.Generic;
    using whateverthefuck.src.model.entities;

    public enum AbilityType
    {
        Fireballx,
    }

    public class Ability
    {
        public Ability(AbilityType abilityType)
        {
            this.AbilityType = abilityType;
        }

        public AbilityType AbilityType { get; private set; }

        public IEnumerable<GameEvent> Resolve(GameEntity caster, GameEntity target)
        {
            List<GameEvent> events = new List<GameEvent>();

            switch (this.AbilityType)
            {
                case AbilityType.Fireballx:
                {
                    events.Add(new DealDamageEvent(caster, target, 15));
                } break;
            }

            return null;
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

        public int CastTicks()
        {
            return 100;
        }
    }

    public class CastingInfo
    {
        public CastingInfo(Ability castingAbility, GameEntity target)
        {
            this.CastingAbility = castingAbility;
            this.Target = target;
            this.MaxTicks = castingAbility.CastTicks();
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
