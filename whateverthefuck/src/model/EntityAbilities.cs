using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    public class EntityAbilities
    {
        private GameEntity Entity;

        public EntityAbilities(GameEntity entity)
        {
            this.Entity = entity;
        }

        public List<Ability> Abilities { get; } = new List<Ability>();

        public CastingInfo Casting { get; set; }

        public int GlobalCooldown { get; private set; }

        public Ability Ability(AbilityType abilityType)
        {
            return this.Abilities.First(a => a.AbilityType == abilityType);
        }

        public IEnumerable<Ability> CastableAbilities(GameEntity target, GameState gameState)
        {
            return this.Abilities.Where(a => this.CanCastAbility(a, target, gameState));
        }

        public bool CanCastAbility(Ability ability, GameEntity target, GameState gameState)
        {
            if (this.GlobalCooldown > 0)
            {
                return false;
            }

            if (ability.CurrentCooldown > 0)
            {
                return false;
            }

            if (!ability.TargetingRule.ApplyRule(this.Entity, target, gameState))
            {
                return false;
            }

            if (ability.Range < this.Entity.DistanceTo(target))
            {
                return false;
            }

            return true;
        }

        public void CastAbility(Ability ability, GameEntity target)
        {
            if (this.Casting != null)
            {
                return;
            }

            if (!this.CanMoveWhileCasting(ability) && this.Entity.Movements.IsMoving)
            {
                return;
            }

            this.GlobalCooldown = this.Entity.Status.ReadCurrentStats.GlobalCooldown;

            ability.CurrentCooldown = ability.BaseCooldown;
            this.Casting = new CastingInfo(ability, target);
        }

        public float CooldownPercentage(Ability ability)
        {
            var globalCooldownPercentage = (float)this.GlobalCooldown / this.Entity.Status.ReadCurrentStats.GlobalCooldown;
            var abilityCooldownPercentage = ability.CooldownPercentage;

            if (this.GlobalCooldown > ability.CurrentCooldown)
            {
                return globalCooldownPercentage;
            }
            else
            {
                return abilityCooldownPercentage;
            }
        }

        public bool CanMoveWhileCasting(Ability ability)
        {
            if (ability.CastTime == 0)
            {
                return true;
            }

            return false;
        }
    }
}
