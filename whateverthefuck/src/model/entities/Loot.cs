namespace whateverthefuck.src.model.entities
{
    using System.Collections.Generic;
    using System.Drawing;
    using whateverthefuck.src.util;

    public class Loot : GameEntity, IInteractable
    {
        public Loot(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.Loot, args)
        {
            this.DrawColor = Color.Gold;
            this.Size = new GameCoordinate(0.05f, 0.05f);
            this.Collidable = false;
            this.Targetable = true;
        }

        public List<Item> Items { get; private set; } = new List<Item>();

        public void Interact()
        {
            Program.GameStateManager.ShowLoot(this);
        }
    }
}
