using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

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

            if (!ability.CanTarget(this.Entity, target, gameState))
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

        public void Step(GameState gameState)
        {

            if (this.Casting != null)
            {
                if (this.Entity.Movements.IsMoving && !this.CanMoveWhileCasting(this.Casting.CastingAbility))
                {
                    // cancel the cast
                    this.Casting = null;
                    // reset global cooldown
                    this.GlobalCooldown = 0;
                }
                else
                {
                    this.Casting.Step();
                    if (this.Casting.DoneCasting)
                    {
                        gameState.HandleGameEvents(new EndCastAbility(
                            this.Entity.Identifier,
                            this.Casting.Target,
                            this.Casting.CastingAbility));

                        this.Casting = null;
                    }
                }
            }

            if (this.GlobalCooldown > 0)
            {
                this.GlobalCooldown--;
            }

            foreach (var a in this.Abilities)
            {
                if (a.CurrentCooldown > 0)
                {
                    a.CurrentCooldown--;
                }
            }
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
