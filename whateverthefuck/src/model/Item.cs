using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Type = type;
            StackSize = stackSize;
            Rarity = rarity;
            Bonuses = bonuses;
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
            get { return (BonusType)(Value & LowMask);  }
            set { Value = (int)((Value & HighMask) | ((int)value & LowMask)); }
        }

        public short Modifier
        {
            get { return (short)((Value & HighMask) >> 16); }
            set { Value = (int)((Value & LowMask) | (((int)value & LowMask) << 16)); }
        }

        public ItemBonus(BonusType type, short value)
        {
            Type = type;
            Modifier = value;
        }

        public ItemBonus(int intValue)
        {
            Value = intValue;
        }

        public int ToInt()
        {
            return Value;
        }
    }
}
