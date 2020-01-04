using OpenTK.Graphics;
using QuickFont;
using System;
using System.Drawing;

namespace whateverthefuck.src.view.guicomponents
{
    internal class TextPanel : Panel
    {
        public string Text { get; set; }

        private QFontRenderOptions RenderOptions { get; }

        public TextPanel(string text) : base()
        {
            this.Text = text;
            this.RenderOptions = new QFontRenderOptions { Colour = Color.Black, DropShadowActive = true };
            this.Size = CalculateSize(this.Text);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);

            var loc = GUI.GLToScreenCoordinates((this.Location as GLCoordinate));
            FontLoader.Drawing.Print(FontLoader.DefaultFont, this.Text, new OpenTK.Vector3(loc.X, loc.Y, 0), QFontAlignment.Left, this.RenderOptions);
        }

        private GLCoordinate CalculateSize(string text)
        {
            var textSize = FontLoader.DefaultFont.Measure(this.Text);
            return GUI.ScreenToGLCoordinates(new ScreenCoordinate(textSize.Width, textSize.Height)) + new GLCoordinate(1, -1);
        }
    }
}
