namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.util;

    internal class LootPanel : Panel
    {
        public LootPanel(GLCoordinate size, Loot loot)
            : base(new GLCoordinate(0, 0), size)
        {
            this.BackColor = Color.RosyBrown;

            var glm = new GridLayoutManager();
            glm.Rows = 1;
            glm.Width = this.Size.X / 6;
            glm.Height = this.Size.Y;
            this.LayoutManager = glm;

            foreach (var item in loot.Items)
            {
                Button lootButton = new Button();
                lootButton.Sprite = item.Sprite;
                lootButton.OnMouseButtonDown += (c, i) => lootButton.Visible = false;
                this.AddChild(lootButton);
            }
        }
    }
}
