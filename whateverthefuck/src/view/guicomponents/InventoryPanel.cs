﻿namespace whateverthefuck.src.view.guicomponents
{
    using whateverthefuck.src.model;

    internal class InventoryPanel : Panel
    {
        private static readonly int NumberOfRows = 3;
        private static readonly int NumberOfColumns = 3;

        private static readonly GLCoordinate Location = new GLCoordinate(0, 0);
        private static readonly GLCoordinate Size = new GLCoordinate(0.5f, 0.5f);
        private static readonly GLCoordinate Padding = new GLCoordinate(0.02f, 0.02f);

        internal InventoryPanel()
            : base(Location,  Size)
        {
            var glm = new GridLayoutManager();
            glm.Rows = NumberOfRows;
            glm.Columns = NumberOfColumns;
            glm.XPadding = Padding.X;
            glm.YPadding = Padding.Y;
            glm.Width = (Size.X - (Padding.X * (glm.Columns + 1))) / glm.Columns;
            glm.Height = (Size.Y - (Padding.Y * (glm.Rows + 1))) / glm.Rows;
            this.LayoutManager = glm;

            this.AddBorder();

            this.Visible = false;
        }

        private static int InventorySize => NumberOfRows * NumberOfColumns;

        public bool AddItem(Item i)
        {
            if (this.Children.Count >= InventorySize)
            {
                return false;
            }

            ItemContainer ic = new ItemContainer(i);
            this.AddChild(ic);

            return true;
        }

        public void Update(Inventory inventory)
        {
            this.ClearChildren();

            foreach (var i in inventory.AllItems)
            {
                this.AddItem(i);
            }
        }
    }
}