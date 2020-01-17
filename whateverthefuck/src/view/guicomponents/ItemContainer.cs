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

            this.AddBorder();
            this.BackColor = RNG.RandomColor();

            this.OnMouseButtonDown += (component, union) =>
            {
                Program.GameStateManager.UseItem(item);
                StacksText.Text = item.StackSize.ToString();
            };

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

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            // @hack
            this.StacksText.Location = new GLCoordinate(this.Location.X, this.Location.Y + this.Size.Y);
            base.DrawMe(drawAdapter);
        }
    }
}
