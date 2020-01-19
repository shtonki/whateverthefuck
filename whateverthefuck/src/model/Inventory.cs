namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using whateverthefuck.src.util;

    public class Inventory : IEncodable
    {
        public event Action OnInventoryChanged;

        public IEnumerable<Item> AllItems => this.Items;

        private List<Item> Items { get; } = new List<Item>();

        public void AddItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public void AddItem(Item item)
        {
            if (item.Stackable)
            {
                // @incomplete check rarity etc
                var existing = Items.FirstOrDefault(i => i.Type == item.Type);

                if (existing != null)
                {
                    existing.StackSize += item.StackSize;
                    this.OnInventoryChanged?.Invoke();
                    return;
                }
            }

            this.Items.Add(item);
            this.OnInventoryChanged?.Invoke();
        }

        public void RemoveItem(Item item)
        {
            if (Items.Remove(item))
            {
            }
            else
            {
                if (item.Stackable)
                {
                    var stack = GetByType(item.Type, item.Rarity);

                    if (stack != null && stack.StackSize >= item.StackSize)
                    {
                        stack.StackSize -= item.StackSize;

                        if (stack.StackSize == 0)
                        {
                            Items.Remove(stack);
                        }

                        OnInventoryChanged?.Invoke();
                    }
                }
                else
                {
                    var identical = GetIdentical(item);
                    if (identical != null)
                    {
                        this.Items.Remove(identical);
                        this.OnInventoryChanged?.Invoke();
                    }
                }
            }

            CleanUp();
        }

        public void Clear()
        {
            this.Items.Clear();
            this.OnInventoryChanged?.Invoke();
        }

        public Item GetByType(ItemType type, Rarity rarity)
        {
            return Items.FirstOrDefault(item => item.Type == type && item.Rarity == rarity);
        }

        public Item GetIdentical(Item item)
        {
            // @incomplete not actually identical
            return AllItems.FirstOrDefault(i => i.Type == item.Type && i.Rarity == item.Rarity);
        }

        public void Encode(WhateverEncoder encoder)
        {
            this.CleanUp();

            encoder.Encode(Items);
        }

        public void Decode(WhateverDecoder decoder)
        {
            var items = decoder.DecodeItems();
            Items.AddRange(items);
        }

        private void CleanUp()
        {
            Items.RemoveAll(i => i.StackSize == 0);
        }
    }
}
