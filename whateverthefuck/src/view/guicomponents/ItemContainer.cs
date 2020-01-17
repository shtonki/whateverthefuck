namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.model;
    using whateverthefuck.src.util;

    internal class ItemContainer : Button
    {
        public ItemContainer(Item item)
        {
            this.Sprite = item.Sprite;
            this.Item = item;

            this.AddBorder(RarityColor(item.Rarity));
            this.BackColor = RNG.RandomColor();

            StacksText = new TextPanel(item.StackSize.ToString(), Color.Black);
            StacksText.Size = new GLCoordinate(Size.X, Size.Y);
            StacksText.Font = FontLoader.HugeFont;
            if (item.StackSize > 1)
            {
                AddChild(StacksText);
            }
        }

        private TextPanel StacksText { get; set; }

        private Item Item { get; }

        public void UpdateStackText()
        {
            this.StacksText.Text = this.Item.StackSize.ToString();
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            // @hack
            this.StacksText.Location = new GLCoordinate(this.Location.X, this.Location.Y + this.Size.Y);
            base.DrawMe(drawAdapter);
        }

        private static Color RarityColor(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.None: return Color.Black;
                case Rarity.Common: return Color.Gray;
                case Rarity.Uncommon: return Color.Green;
                case Rarity.Rare: return Color.Blue;
                case Rarity.Epic: return Color.Purple;
                case Rarity.Legendary: return Color.Orange;
                default: return Color.White;
            }
        }
    }
}
