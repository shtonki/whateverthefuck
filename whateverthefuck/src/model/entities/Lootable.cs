﻿namespace whateverthefuck.src.model.entities
{
    using System.Collections.Generic;
    using System.Drawing;
    using whateverthefuck.src.util;

    public class Lootable : GameEntity, IInteractable
    {
        public Lootable(EntityIdentifier identifier, EntityType type, CreationArgs args)
            : base(identifier, type, args)
        {
        }

        public IEnumerable<Item> Items => this.Loot.Items;

        private Loot Loot { get; } = new Loot();

        public void Interact()
        {
            if (this.Loot.Items.Count > 0)
            {
                Program.GameStateManager.Loot(this);
            }
        }

        public void AddLoot(Item item)
        {
            this.Loot.Items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            this.Loot.Items.Remove(item);
        }
    }

    public class Loot
    {
        public Loot()
        {
        }

        public List<Item> Items { get; private set; } = new List<Item>();
    }
}