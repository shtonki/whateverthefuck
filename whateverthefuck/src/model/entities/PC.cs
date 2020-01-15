namespace whateverthefuck.src.model.entities
{
    using System;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class PC : Character
    {
        public PC(EntityIdentifier identifier, CreationArguments args)
            : base(identifier, EntityType.PC, args)
        {
            this.DrawColor = Coloring.RandomColor();

            this.Status.BaseStats.MoveSpeed = 0.01f;
            this.Status.BaseStats.MaxHealth = 100;

            this.Abilities.Abilities.Add(new Mend());
            this.Abilities.Abilities.Add(new Fireball());
            this.Abilities.Abilities.Add(new Fireburst());

            this.Sprite = new Sprite(SpriteID.player_Player0);
        }
    }

    public class PCCreationArguments : CreationArguments
    {
        public override void Encode(WhateverEncoder encoder)
        {
        }

        public override void Decode(WhateverDecoder decoder)
        {
        }
    }
}
