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

            StatStruct baseStats = new StatStruct();
            baseStats.MaxHealth = 100;
            baseStats.GlobalCooldown = 100;
            baseStats.MoveSpeed = Character.SpeedSlow;

            this.Status = new EntityStatus(baseStats);

            this.Abilities.Abilities.Add(new Mend());
            this.Abilities.Abilities.Add(new Fireball());
            this.Abilities.Abilities.Add(new Fireburst());

            this.Sprite = new Sprite(SpriteID.player_Player0);

            this.Equipment = new Equipment();
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
