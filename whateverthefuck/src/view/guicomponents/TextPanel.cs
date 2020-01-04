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
            this.Size = new GLCoordinate(0.4f, 0.2f);
            this.Location = new GLCoordinate(0, 0);
            this.Text = text;
            this.RenderOptions = new QFontRenderOptions { Colour = Color.Black, DropShadowActive = true };
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
            FontLoader.Drawing.Print(FontLoader.DefaultFont, this.Text, new OpenTK.Vector3(100, 100, 0), QFontAlignment.Left, this.RenderOptions);
        }
    }
}
