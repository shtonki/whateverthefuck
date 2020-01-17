using System;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck
{
    public class Equipment
    {
        public event Action<EquipmentSlots, Item> OnEquipmentChanged;

        private Item[] Equipped { get; } = new Item[Enum.GetValues(typeof(EquipmentSlots)).Length];

        public void ApplyStaticEffects(StatStruct stats)
        {
            for (int i = 0; i < Equipped.Length; i++)
            {
                var item = Equipped[i];

                if (item == null)
                {
                    continue;
                }

                item.OnEquip(stats);
            }
        }

        public void Equip(Item item)
        {
            if (!item.Equipable)
            {
                Logging.Log("Tried to equip unequipable item.", Logging.LoggingLevel.Error);
            }

            Equipped[(int)item.EquipmentSlot] = item;
            OnEquipmentChanged?.Invoke(item.EquipmentSlot, item);
        }

        public Item GetItem(EquipmentSlots slot)
        {
            return Equipped[(int)slot];
        }
    }

    public enum EquipmentSlots
    {
        None,

        Head,
        MainHand,
    }
}
