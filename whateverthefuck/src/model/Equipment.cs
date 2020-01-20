using System;
using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public class Equipment : IEncodable
    {
        public event Action<EquipmentSlots, Item> OnEquipmentChanged;

        public IEnumerable<Item> Equipped => this.ItemArray.Where(item => item != null);

        private Item[] ItemArray { get; } = new Item[Enum.GetValues(typeof(EquipmentSlots)).Length];

        public void ApplyStaticEffects(StatStruct stats)
        {
            for (int i = 0; i < ItemArray.Length; i++)
            {
                var item = ItemArray[i];

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

            ItemArray[(int)item.EquipmentSlot] = item;
            OnEquipmentChanged?.Invoke(item.EquipmentSlot, item);
        }

        public Item GetItem(EquipmentSlots slot)
        {
            return ItemArray[(int)slot];
        }

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(ItemArray);
        }

        public void Decode(WhateverDecoder decoder)
        {
            var items = decoder.DecodeItemArray();

            for (int i = 0; i < ItemArray.Length; i++)
            {
                ItemArray[i] = items[i];
            }
        }
    }

    public enum EquipmentSlots
    {
        None,

        Head,
        MainHand,
    }
}
