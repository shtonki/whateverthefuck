namespace whateverthefuck.src.view.guicomponents
{
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
                Logging.Log("Click down", Logging.LoggingLevel.Fatal);
            };
        }

        private Item Item { get; }
    }
}
