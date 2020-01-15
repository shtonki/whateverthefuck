using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    internal class TargetPanel : Panel
    {
        private const float EntityPortraitPadding = 0.05f;
        private const float HealthThickness = 0.1f;

        private GameEntity entity;
        private QFontRenderOptions renderOptions;

        public TargetPanel(GameEntity entity)
        {
            this.entity = entity;
            this.renderOptions = new QFontRenderOptions { Colour = Color.Black, DropShadowActive = true };

        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);

            drawAdapter.DrawSprite(
                EntityPortraitPadding,
                EntityPortraitPadding,
                this.Size.Y - (2 * EntityPortraitPadding),
                this.Size.Y - (2 * EntityPortraitPadding),
                this.entity.Sprite);

            var healthX0 = this.Size.Y;
            var healthX1 = this.Size.X;
            var healthY0 = this.Size.Y - HealthThickness;
            var healthY1 = this.Size.Y;

            if (this.entity.State == GameEntityState.Dead)
            {
                drawAdapter.FillRectangle(healthX0, healthY0, healthX1, healthY1, Color.Gray);
            }
            else
            {
                var healthPct = (float)this.entity.Status.ReadCurrentStats.Health / this.entity.Status.ReadCurrentStats.MaxHealth;
                var healthWidth = this.Size.X - this.Size.Y;
                var healthXMiddle = healthX0 + (healthPct * healthWidth);

                drawAdapter.FillRectangle(healthX0, healthY0, healthXMiddle, healthY1, Color.Green);
                drawAdapter.FillRectangle(healthXMiddle, healthY0, healthX1, healthY1, Color.Red);
            }

            if (this.entity.Abilities.Casting != null)
            {
                var casting = this.entity.Abilities.Casting;

                var sprite = Sprite.GetAbilitySprite(casting.CastingAbility.AbilityType);
                var castingX0 = this.Size.Y;
                var castingY0 = 0;
                var castingX1 = this.Size.Y + this.Size.Y - HealthThickness;
                var castingY1 = this.Size.Y - HealthThickness;

                drawAdapter.DrawSprite(castingX0, castingY0, castingX1, castingY1, sprite);
            }

            drawAdapter.DrawText(FontLoader.DefaultFont, "Entity Name", new GLCoordinate(this.Location.X + this.Size.Y, this.Location.Y + this.Size.Y), QuickFont.QFontAlignment.Centre, this.renderOptions);
        }
    }
}
