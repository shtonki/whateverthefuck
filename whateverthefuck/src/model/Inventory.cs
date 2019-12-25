namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;

    public class Inventory
    {
        public event Action OnInventoryChanged;

        public IEnumerable<Item> AllItems => this.Items;

        private List<Item> Items { get; } = new List<Item>();

        public void AddItem(Item i)
        {
            this.Items.Add(i);
            this.OnInventoryChanged?.Invoke();
        }
    }
}
