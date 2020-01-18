namespace whateverthefuck.src.model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;
    using whateverthefuck.src.view.guicomponents;

    public abstract class Item : IEncodable, ToolTipper
    {
        protected Item(ItemType type, Rarity rarity)
        {
            Type = type;
            Rarity = rarity;
        }

        public static Item FromDecoder(WhateverDecoder decoder)
        {
            var type = (ItemType)decoder.DecodeInt();

            if (type == ItemType.None) { return null; }

            var item = FromType(type);
            item.Decode(decoder);

            return item;
        }

        private static Item FromType(ItemType type)
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

                case ItemType.BronzeHelmet:
                {
                    return new BronzeHelmet();
                }

                default: throw new Exception(type.ToString());
            }
        }

        public ItemType Type { get; private set; }

        public int StackSize { get; set; } = 1;

        public bool Stackable { get; protected set; }

        public Rarity Rarity { get; private set; }

        public EquipmentSlots EquipmentSlot { get; protected set; }

        public ItemBonus[] Bonuses { get; protected set; } = new ItemBonus[0];

        public bool DepletesOnUse { get; protected set; }

        public Sprite Sprite => Sprite.GetItemSprite(this);

        public bool Equipable => EquipmentSlot != EquipmentSlots.None;

        public virtual IEnumerable<GameEvent> OnUse(GameEntity target)
        {
            return null;
        }

        public void OnEquip(StatStruct stats)
        {
            foreach (var bonus in Bonuses)
            {
                bonus.Apply(stats);
            }
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

        protected static int ValueFromRarity(Rarity rarity, int common, int uncommon, int rare, int epic, int legendary)
        {
            switch (rarity)
            {
                case Rarity.Common: return common;
                case Rarity.Uncommon: return uncommon;
                case Rarity.Rare: return rare;
                case Rarity.Epic: return epic;
                case Rarity.Legendary: return legendary;

                default: return 0;
            }
        }

        public string GetToolTip()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Type.ToString());
            sb.Append(Environment.NewLine);

            if (EquipmentSlot != EquipmentSlots.None)
            {
                sb.Append(EquipmentSlot.ToString());
                sb.Append(Environment.NewLine);
            }

            sb.Append(Rarity.ToString());
            sb.Append(Environment.NewLine);

            foreach (var bonus in Bonuses)
            {
                sb.Append(bonus.GetToolTip());
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }

    public class BronzeHelmet : Item
    {
        public BronzeHelmet()
            : this(Rarity.None)
        {
        }

        public BronzeHelmet(Rarity rarity)
            : base(ItemType.BronzeHelmet, rarity)
        {
            EquipmentSlot = EquipmentSlots.Head;

            Bonuses = new ItemBonus[]
            {
                new ItemBonus(ItemBonus.BonusType.BonusIntelligence, ValueFromRarity(rarity, 5, 10, 15, 20, 25)),
            };
        }

    }

    public class BronzeDagger : Item
    {
        public BronzeDagger()
            : this(Rarity.None)
        {
        }

        public BronzeDagger(Rarity rarity)
            : base(ItemType.BronzeDagger, rarity)
        {
            EquipmentSlot = EquipmentSlots.MainHand;

            Bonuses = new ItemBonus[]
            {
                new ItemBonus(ItemBonus.BonusType.BonusIntelligence, ValueFromRarity(rarity, 5, 10, 15, 20, 25)),
            };
        }
    }

    public class Banana : Item
    {
        public Banana()
            : this(Rarity.None, 0)
        {
        }

        public Banana(Rarity rarity, int stackSize)
            : base(ItemType.Banana, rarity)
        {
            Stackable = true;
            DepletesOnUse = true;

            StackSize = stackSize;
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

        BronzeHelmet,

        Banana,
    }

    public class ItemBonus : IEncodable, ToolTipper
    {
        public ItemBonus(BonusType type, int value)
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

            MoveSpeed,

            BonusIntelligence,
        }

        public BonusType Type { get; private set; }

        public int Modifier { get; private set; }

        public void Apply(StatStruct stats)
        {
            switch (Type)
            {
                case BonusType.MoveSpeed:
                {
                    stats.MoveSpeed *= 1 + (0.01f * Modifier);
                } break;

                case BonusType.BonusIntelligence:
                {
                    stats.Intelligence += Modifier;
                } break;

                default:
                {
                    Logging.Log("Can't apply ItemBonus type: " + Type);
                } break;
            }
        }

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

        public string GetToolTip()
        {
            switch (Type)
            {
                case BonusType.BonusIntelligence: return String.Format("+{0} Intelligence", Modifier);
                case BonusType.MoveSpeed: return String.Format("+{0}% Move Speed", Modifier);

                default: return "Tooltip missing...";
            }
        }
    }
}
