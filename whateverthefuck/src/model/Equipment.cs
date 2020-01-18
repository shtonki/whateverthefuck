using System;
using System.Collections.Generic;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model
{
    public class Equipment : IEncodable
    {
        public event Action<EquipmentSlots, Item> OnEquipmentChanged;

        public IEnumerable<Item> Items => this.Equipped;

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

        public void Encode(WhateverEncoder encoder)
        {
            for (int i = 0; i < Equipped.Length; i++)
            {
                var item = Equipped[i];

                if (item == null)
                {
                    encoder.Encode((int)EquipmentSlots.None);
                }
                else
                {
                    item.Encode(encoder);
                }
            }
            throw new NotImplementedException();
        }

        public void Decode(WhateverDecoder decoder)
        {
            var items = decoder.DecodeItems();

            for (int i = 0; i < Equipped.Length; i++)
            {
                Equipped[i] = items[i];
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
