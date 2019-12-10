using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    public class Ability
    {
        public Abilities AbilityType { get; private set; }

        public Ability(Abilities abilityType)
        {
            AbilityType = abilityType;
        }
    }

    public enum Abilities
    {
        Fireballx,
    }
}
