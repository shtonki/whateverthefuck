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

            this.Sprite = new Sprite(args.GetSpriteID());
        }
    }

    public class NPCCreationArguments : CreationArguments
    {
        public NPCCreationArguments(Types type)
        {
            this.Type = type;
        }

        public NPCCreationArguments()
        {

        }

        public enum Types
        {
            Dog,
        }

        public Types Type { get; private set; }

        public SpriteID GetSpriteID()
        {
            switch (this.Type)
            {
                case Types.Dog:
                {
                    return SpriteID.npc_dog;
                }

                default: return SpriteID.testSprite1;
            }
        }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.Type);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Type = (Types)decoder.DecodeInt();
        }
    }
}
