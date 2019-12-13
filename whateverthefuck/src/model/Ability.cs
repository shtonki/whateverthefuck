namespace whateverthefuck.src.model
{
    public enum Abilities
    {
        Fireballx,
    }

    public class Ability
    {
        public Ability(Abilities abilityType)
        {
            this.AbilityType = abilityType;
        }

        public Abilities AbilityType { get; private set; }
    }
}
