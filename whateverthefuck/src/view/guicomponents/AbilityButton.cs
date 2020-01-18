namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.model;

    internal class AbilityButton : Button, ToolTipper
    {
        private string tooltip;

        public AbilityButton(Ability a)
        {
            this.Sprite = Sprite.GetAbilitySprite(a.AbilityType);
            tooltip = a.GetToolTip();
        }

        public float CooldownPercentage { get; set; }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
            drawAdapter.FillRectangle(0, 0, this.Size.X, this.Size.Y * this.CooldownPercentage, Color.FromArgb(100, 100, 100, 100));
        }

        public string GetToolTip()
        {
            return tooltip;
        }
    }
}
