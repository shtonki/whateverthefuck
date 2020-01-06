using OpenTK.Graphics;
using QuickFont;
using System;
using System.Drawing;

namespace whateverthefuck.src.view.guicomponents
{
    internal class TextPanel : Panel
    {
        public TextPanel(string text) : base()
        {
            this.Text = text;
            this.RenderOptions = new QFontRenderOptions { Colour = Color.Black, DropShadowActive = true };
            this.Size = this.CalculateSize(this.Text);
        }

        public string Text { get; set; }

        private QFontRenderOptions RenderOptions { get; }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);

            var loc = GUI.GLToScreenCoordinates(this.Location as GLCoordinate);
            drawAdapter.DrawText(FontLoader.DefaultFont, this.Text, this.Location as GLCoordinate, QFontAlignment.Left, this.RenderOptions);
        }

        private GLCoordinate CalculateSize(string text)
        {
            var textSize = FontLoader.DefaultFont.Measure(this.Text);
            return GUI.ScreenToGLCoordinates(new ScreenCoordinate(textSize.Width, textSize.Height)) + new GLCoordinate(1, -1);
        }
    }
}
