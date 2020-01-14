namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    public class NPC : Character
    {
        public NPC(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.NPC, args)
        {
            this.DrawColor = Color.Red;
            this.Movable = true;

            this.Status.BaseStats.MaxHealth = 100;
            this.Status.BaseStats.MoveSpeed = 0.001f;

            this.Abilities.Abilities.Add(new Ability(AbilityType.Bite));
        }
    }
}
