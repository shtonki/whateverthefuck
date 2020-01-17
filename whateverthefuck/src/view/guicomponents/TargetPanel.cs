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
        private const int StatusRows = 3;
        private const int StatusColumns = 4;
        private const float StatusPadding = 0.01f;

        private const float EntityPortraitPadding = 0.05f;
        private const float HealthThickness = 0.1f;

        private GameEntity entity;
        private QFontRenderOptions nameRenderOptions;
        private QFontRenderOptions statusStackCountRenderOptions;

        public TargetPanel(GameEntity entity)
        {
            this.entity = entity;
            this.nameRenderOptions = new QFontRenderOptions { Colour = Color.Black, DropShadowActive = true };
            this.statusStackCountRenderOptions = new QFontRenderOptions { Colour = Color.Orange, DropShadowActive = true };

        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);

            var portraitX0 = EntityPortraitPadding;
            var portraitY0 = EntityPortraitPadding;
            var portraitX1 = this.Size.Y - (2 * EntityPortraitPadding);
            var portraitY1 = this.Size.Y - (2 * EntityPortraitPadding);

            drawAdapter.DrawSprite(
                portraitX0,
                portraitY0,
                portraitX1,
                portraitY1,
                this.entity.Sprite);

            var healthX0 = this.Size.Y;
            var healthX1 = this.Size.X;
            var healthY0 = this.Size.Y - HealthThickness;
            var healthY1 = this.Size.Y;

            if (this.entity.Info.State == GameEntityState.Dead)
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

            var castingX1 = this.Size.Y + this.Size.Y - HealthThickness;

            if (this.entity.Abilities.Casting != null)
            {
                var casting = this.entity.Abilities.Casting;

                var sprite = Sprite.GetAbilitySprite(casting.CastingAbility.AbilityType);
                var castingX0 = this.Size.Y;
                var castingY0 = 0;
                var castingY1 = this.Size.Y - HealthThickness;

                drawAdapter.DrawSprite(castingX0, castingY0, castingX1, castingY1, sprite);
            }

            drawAdapter.DrawText(FontLoader.DefaultFont, this.entity.Info.Level.ToString(), new GLCoordinate(this.Location.X + this.Size.Y, this.Location.Y + this.Size.Y), QuickFont.QFontAlignment.Centre, this.nameRenderOptions);

            var statusX0 = castingX1;
            var statusY0 = 0;
            var statusX1 = this.Size.X;
            var statusY1 = healthY0;
            var statusTotalWidth = statusX1 - statusX0;
            var statusTotalHeight = statusY1 - statusY0;
            var statusWidth = statusTotalWidth / StatusColumns;
            var statusHeight = statusTotalHeight / StatusRows;

            var statuses = this.entity.Status.ActiveStatuses;

            for (int i = 0; i < statuses.Count; i++)
            {
                var status = statuses[i];

                int row = i / StatusColumns;
                int column = i % StatusColumns;
                var X0 = statusX0 + (column * statusWidth);
                var Y0 = statusY0 + (row * statusHeight);
                var X1 = X0 + statusWidth;
                var Y1 = Y0 + statusHeight;
                var sprite = status.Sprite;

                drawAdapter.DrawSprite(X0, Y0, X1, Y1, sprite);
                drawAdapter.DrawText(FontLoader.BigFont, status.Stacks.ToString(), new GLCoordinate(this.Location.X + X0, this.Location.Y + Y1 + (statusHeight / 3)), QFontAlignment.Left, this.statusStackCountRenderOptions);
            }
        }
    }
}
