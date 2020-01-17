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
            this.Info.Level = args.Level;

            this.Status = new EntityStatus(args.GetBaseStats());

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

        public NPCCreationArguments(Types type, int level, int random)
        {
            this.Type = type;
            this.Level = level;
            this.Random = random;
        }

        public enum Types
        {
            Dog,
        }

        public Types Type { get; private set; }

        public int Level { get; private set; }

        public int Random { get; private set; }

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

        public StatStruct GetBaseStats()
        {
            StatStruct baseStats = new StatStruct();

            baseStats.MaxHealth = 100 * this.Level;
            baseStats.MoveSpeed = Character.SpeedFast;

            baseStats.Strength = this.Level;

            return baseStats;
        }

        public override void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.Type);
            encoder.Encode(this.Level);
            encoder.Encode(this.Random);
        }

        public override void Decode(WhateverDecoder decoder)
        {
            this.Type = (Types)decoder.DecodeInt();
            this.Level = decoder.DecodeInt();
            this.Random = decoder.DecodeInt();
        }
    }
}
