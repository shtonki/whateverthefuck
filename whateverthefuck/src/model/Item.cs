namespace whateverthefuck.src.model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public abstract class Item : IEncodable
    {
        protected Item(ItemType type)
        {
            Type = type;
        }

        public static Item FromDecoder(WhateverDecoder decoder)
        {
            var type = (ItemType)decoder.DecodeInt();
            var item = FromType(type);
            item.Decode(decoder);

            return item;
        }

        public static Item FromType(ItemType type)
        {
            switch (type)
            {
                case ItemType.Banana:
                {
                    return new Banana();
                }

                case ItemType.BronzeDagger:
                {
                    return new BronzeDagger();
                }

                default: throw new Exception(type.ToString());
            }
        }

        public ItemType Type { get; private set; }

        public int StackSize { get; set; }

        public bool Stackable { get; set; }

        public Rarity Rarity { get; set; }

        public ItemBonus[] Bonuses { get; set; } = new ItemBonus[0];

        public bool DepletesOnUse { get; protected set; }

        public Sprite Sprite => Sprite.GetItemSprite(this);

        public virtual IEnumerable<GameEvent> OnUse(GameEntity target)
        {
            return null;
        }

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)this.Type);
            encoder.Encode((int)this.Rarity);
            encoder.Encode(this.StackSize);
            encoder.Encode(this.Bonuses.Length);
            foreach (var bonus in this.Bonuses)
            {
                bonus.Encode(encoder);
            }
        }

        public void Decode(WhateverDecoder decoder)
        {
            // @warning this doesn't eat Type like it's supposed to so it isn't used symmetrically with Encode.
            this.Rarity = (Rarity)decoder.DecodeInt();
            this.StackSize = decoder.DecodeInt();
            this.Bonuses = new ItemBonus[decoder.DecodeInt()];

            for (int i = 0; i < this.Bonuses.Length; i++)
            {
                ItemBonus bonus = new ItemBonus();
                bonus.Decode(decoder);
                this.Bonuses[i] = bonus;
            }
        }
    }

    public class BronzeDagger : Item
    {
        public BronzeDagger()
            : base(ItemType.BronzeDagger)
        {
            StackSize = 1;
        }
    }

    public class Banana : Item
    {
        public Banana()
            : base(ItemType.Banana)
        {
            Stackable = true;
            DepletesOnUse = true;
        }

        public override IEnumerable<GameEvent> OnUse(GameEntity target)
        {
            return new GameEvent[] { new HealEvent(target, target, 20) };
        }
    }

    public enum Rarity
    {
        None,

        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
    }

    public enum ItemType
    {
        None,

        BronzeDagger,

        Banana,
    }

    public class ItemBonus : IEncodable
    {
        public ItemBonus(BonusType type, short value)
        {
            this.Type = type;
            this.Modifier = value;
        }

        public ItemBonus()
        {
        }

        public enum BonusType
        {
            None,

            Test1,
            Test2,
            Test3,
            Test4,
        }

        public BonusType Type { get; private set; }

        public int Modifier { get; private set; }

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode((int)Type);
            encoder.Encode(Modifier);
        }

        public void Decode(WhateverDecoder decoder)
        {
            this.Type = (BonusType)decoder.DecodeInt();
            this.Modifier = decoder.DecodeInt();
        }
    }
}
