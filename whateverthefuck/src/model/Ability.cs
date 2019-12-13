namespace whateverthefuck.src.model
{
    /// <summary>
    /// The enum of all Abilities in the game.
    /// </summary>
    public enum Abilities
    {
        Fireballx,
    }

    /// <summary>
    /// Represents an Ability in the game.
    /// </summary>
    public class Ability
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ability"/> class.
        /// </summary>
        /// <param name="abilityType">The type of Ability being created.</param>
        public Ability(Abilities abilityType)
        {
            this.AbilityType = abilityType;
        }

        /// <summary>
        /// Gets the type of Ability.
        /// </summary>
        public Abilities AbilityType { get; private set; }
    }

}
