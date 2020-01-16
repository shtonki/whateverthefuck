namespace whateverthefuck.src.model.entities
{
    using System.Drawing;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public class NPC : Character
    {
        public NPC(EntityIdentifier id, CreationArguments args)
            : this(id, args as NPCCreationArguments)
        {
        }

        public NPC(EntityIdentifier identifier, NPCCreationArguments args)
            : base(identifier, EntityType.NPC, args)
        {
            this.DrawColor = Color.Red;
            this.Info.Movable = true;

            this.Status.BaseStats.MaxHealth = 100;
            this.Status.BaseStats.MoveSpeed = 0.015f;

            this.Abilities.Abilities.Add(new Bite());
            this.Abilities.Abilities.Add(new Mend());

            this.Sprite = new Sprite(args.GetSpriteID());
        }
    }

    public class NPCCreationArguments : CreationArguments
    {
        public NPCCreationArguments()
        {

        }

        public NPCCreationArguments(Types type, int level)
        {
            this.Type = type;
            this.Level = level;
        }

        public enum Types
        {
            Dog,
        }

        public Types Type { get; private set; }

        public int Level { get; private set; }

        public SpriteID GetSpriteID()
        {
            switch (this.Type)
            {
                case Types.Dog:
                {
                    return SpriteID.npc_Dog;
                }

                default: return SpriteID.testSprite1;
            }
        }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.Type);
            encoder.Encode(this.Level);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Type = (Types)decoder.DecodeInt();
            this.Level = decoder.DecodeInt();
        }
    }
}
