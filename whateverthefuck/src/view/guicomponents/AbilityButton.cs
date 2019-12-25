using System.Drawing;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view.guicomponents
{

    internal class AbilityButton : Button
    {
        public AbilityButton(Ability a)
        {
            this.Sprite = Sprite.GetAbilitySprite(a.AbilityType);
        }

        public float CooldownPercentage { get; set; }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
            drawAdapter.FillRectangle(0, 0, this.Size.X, this.Size.Y * this.CooldownPercentage, Color.FromArgb(100, 100, 100, 100));
        }
    }
}
