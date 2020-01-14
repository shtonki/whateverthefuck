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

            this.Status.BaseStats.MoveSpeed = 0.01f;

            this.Abilities.Abilities.Add(new Sanic());
            this.Abilities.Abilities.Add(new Fireball());
            this.Abilities.Abilities.Add(new Fireburst());

            this.Sprite = new Sprite(SpriteID.player_Player0);
        }
    }
}
