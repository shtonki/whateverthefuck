namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.model.entities;

    // @todo: use ItemContainer instead of plain button
    internal class LootPanel : Panel
    {
        public LootPanel(GLCoordinate size, Lootable lootee)
            : base(new GLCoordinate(0, 0), size)
        {
            this.BackColor = Color.RosyBrown;

            var glm = new GridLayoutManager();
            glm.Rows = 1;
            glm.Width = this.Size.X / 6;
            glm.Height = this.Size.Y;
            this.LayoutManager = glm;

            foreach (var item in lootee.Items)
            {
                ItemContainer lootButton = new ItemContainer(item);
                lootButton.OnMouseButtonDown += (c, i) =>
                {
                    lootButton.Visible = false;
                    Program.GameStateManager.LootItem(lootee, item);
                };
                this.AddChild(lootButton);
            }
        }
    }
}
