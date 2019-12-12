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

        public BonusType Type { get; }

        public ItemBonus(BonusType type)
        {
            Type = type;
        }

        public ItemBonus(int intValue) : this((BonusType)intValue)
        {

        }

        public int ToInt()
        {
            return (int)Type;
        }
    }
}
