namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.model;

    class CastBar : GUIComponent
    {
        private GameEntity caster;

        public CastBar(GameEntity caster)
        {
            this.caster = caster;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            if (this.caster.Abilities.Casting != null)
            {
                base.DrawMe(drawAdapter);

                var casting = this.caster.Abilities.Casting;

                var abilitySpriteX0 = 0;
                var abilitySpriteY0 = 0;
                var abilitySpriteX1 = this.Size.Y;
                var abilitySpriteY1 = this.Size.Y;

                var sprite = Sprite.GetAbilitySprite(casting.CastingAbility.AbilityType);
                drawAdapter.DrawSprite(0, 0, this.Size.Y, this.Size.Y, sprite);

                var castingCompletionPercentage = casting.PercentageDone;
                var castingBarX0 = abilitySpriteX1;
                var castingBarY0 = this.Size.Y / 3;
                var castingBarX1 = this.Size.X;
                var castingBarY1 = 2 * this.Size.Y / 3;
                var castingBarMiddle = castingBarX0 + ((this.Size.X - castingBarX0) * castingCompletionPercentage);

                drawAdapter.FillRectangle(castingBarX0, castingBarY0, castingBarMiddle, castingBarY1, Color.Yellow);
                drawAdapter.FillRectangle(castingBarMiddle, castingBarY0, castingBarX1, castingBarY1, Color.Black);
            }
        }
    }
}
