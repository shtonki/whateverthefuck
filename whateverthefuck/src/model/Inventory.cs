namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Inventory
    {
        public event Action OnInventoryChanged;

        public IEnumerable<Item> AllItems => this.Items;

        private List<Item> Items { get; } = new List<Item>();

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
            this.Items.Remove(item);
            this.OnInventoryChanged?.Invoke();
        }

        public Item GetByType(ItemType type)
        {
            return AllItems.FirstOrDefault(i => i.Type == type);
        }
    }
}
