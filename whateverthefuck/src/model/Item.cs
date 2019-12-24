using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    public class Item
    {
        public Item(ItemType type, int stackSize, Rarity rarity, params ItemBonus[] bonuses)
        {
            this.Type = type;
            this.StackSize = stackSize;
            this.Rarity = rarity;
            this.Bonuses = bonuses;
        }

        public ItemType Type { get; private set; }

        public int StackSize { get; private set; }

        public Rarity Rarity { get; private set; }

        public ItemBonus[] Bonuses { get; private set; }

        public Sprite Sprite => Sprite.GetItemSprite(this);
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

    public class ItemBonus
    {
        private const uint HighMask = 0xFFFF0000;
        private const uint LowMask = 0x0000FFFF;
        private int value;

        public ItemBonus(BonusType type, short value)
        {
            this.Type = type;
            this.Modifier = value;
        }

        public ItemBonus(int intValue)
        {
            this.value = intValue;
        }

        public enum BonusType
        {
            None,

            Test1,
            Test2,
            Test3,
            Test4,
        }

        public BonusType Type
        {
            get { return (BonusType)(this.value & LowMask);  }
            set { this.value = (int)((this.value & HighMask) | ((int)value & LowMask)); }
        }

        public short Modifier
        {
            get { return (short)((this.value & HighMask) >> 16); }
            set { this.value = (int)((this.value & LowMask) | (((int)value & LowMask) << 16)); }
        }

        public int ToInt()
        {
            return this.value;
        }
    }
}
