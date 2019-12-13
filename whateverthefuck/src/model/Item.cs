namespace whateverthefuck.src.model
{
    public class Item
    {
        public ItemType Type { get; private set; }

        public int StackSize { get; private set; }

        public Rarity Rarity { get; private set; }

        public ItemBonus[] Bonuses { get; private set; }

        public Item(ItemType type, int stackSize, Rarity rarity, params ItemBonus[] bonuses)
        {
            this.Type = type;
            this.StackSize = stackSize;
            this.Rarity = rarity;
            this.Bonuses = bonuses;
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

        Test1,
        Test2,
        Test3,
        Test4,
        Test5,
        Test6,
        Test7,

        BronzeDagger,
    }

    public class ItemBonus
    {
        public enum BonusType
        {
            None,

            Test1,
            Test2,
            Test3,
            Test4,
        }

        private int Value;

        private const uint HighMask = 0xFFFF0000;
        private const uint LowMask = 0x0000FFFF;

        public BonusType Type
        {
            get { return (BonusType)(this.Value & LowMask);  }
            set { this.Value = (int)((this.Value & HighMask) | ((int)value & LowMask)); }
        }

        public short Modifier
        {
            get { return (short)((this.Value & HighMask) >> 16); }
            set { this.Value = (int)((this.Value & LowMask) | (((int)value & LowMask) << 16)); }
        }

        public ItemBonus(BonusType type, short value)
        {
            this.Type = type;
            this.Modifier = value;
        }

        public ItemBonus(int intValue)
        {
            this.Value = intValue;
        }

        public int ToInt()
        {
            return this.Value;
        }
    }
}
