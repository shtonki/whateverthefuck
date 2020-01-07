namespace whateverthefuck.src.model.entities
{
    using System;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class PlayerCharacter : Character
    {
        public PlayerCharacter(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.PlayerCharacter, args)
        {
            this.DrawColor = Coloring.RandomColor();
            this.Sprite = new Sprite(SpriteID.testSprite1);

            this.BaseStats.MoveSpeed = 0.01f;

            this.abilities.Add(new Ability(AbilityType.Fireburst));
            this.abilities.Add(new Ability(AbilityType.Fireball));
            this.abilities.Add(new Ability(AbilityType.Sanic));
        }
    }
}
