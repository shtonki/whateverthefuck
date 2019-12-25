namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;

    public class Inventory
    {
        public event Action OnInventoryChanged;

        private List<Item> Items = new List<Item>();

        public IEnumerable<Item> AllItems => this.Items;

        public Inventory()
        {

        }

        public void AddItem(Item i)
        {
            this.Items.Add(i);
            this.OnInventoryChanged?.Invoke();
        }
    }
}
