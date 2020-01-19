﻿namespace whateverthefuck.src.view.guicomponents
{
    using System;
    using System.Drawing;
    using QuickFont;
    using whateverthefuck.src.model;

    internal class TextPanel : Panel
    {
        public TextPanel(string text, Color color)
            : base()
        {
            this.Text = text;
            this.RenderOptions = new QFontRenderOptions { Colour = color, DropShadowActive = true };
            this.Size = this.CalculateSize(this.Text + Environment.NewLine + Environment.NewLine);
            // we get negative height for some reason i don't really wanna find out
            this.Size.Y = Math.Abs(this.Size.Y);
            this.BackColor = Color.Transparent;
            Font = FontLoader.DefaultFont;
        }

        public string Text { get; set; }

        public QFont Font { get; set; }

        private QFontRenderOptions RenderOptions { get; }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);

            drawAdapter.DrawText(this.Font, this.Text, new GLCoordinate(0, this.Size.Y), QFontAlignment.Left, this.RenderOptions);
        }

        private GLCoordinate CalculateSize(string text)
        {
            var textSize = FontLoader.DefaultFont.Measure(this.Text);
            return GUI.ScreenToGLCoordinates(new ScreenCoordinate(textSize.Width, textSize.Height)) + new GLCoordinate(1, -1);
        }
    }

    internal class DamageTextPanel : TextPanel
    {
        private const int MaxDuration = 80;

        private int counter;
        private GameCoordinate entityLocation;

        public DamageTextPanel(string text, Color color, GameCoordinate location)
            : base(text, color)
        {
            this.entityLocation = location;
        }

        public override void Step()
        {
            // @fucked this doesn't actually remove the damn thing so it's going ot be floating around for all eternity
            if (this.counter++ > MaxDuration)
            {
                Destroy();
            }

            this.Location = Program.GameStateManager.GameState.CurrentCamera.GameToGLCoordinate(this.entityLocation) + new GLCoordinate(0, 0.005f * this.counter);
        }
    }
}
