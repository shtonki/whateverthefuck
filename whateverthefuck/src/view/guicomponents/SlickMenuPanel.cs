using System;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using OpenTK.Graphics.OpenGL4;

namespace whateverthefuck.src.view.guicomponents
{
    using whateverthefuck.src.util;

    internal class SlickMenuPanel : Panel
    {
        public SlickMenuPanel() 
            : base(new GLCoordinate(-0.5f, 0), new GLCoordinate(0.4f, 0.4f))
        {
            this.BackColor = Color.Black;
            this.Visible = true;
            this.OnMouseMove += (component, union) =>
            {
                Logging.Log("hovering this shiznit");
            };

            Button b = new Button(new GLCoordinate(0, 0), new GLCoordinate(0.05f, 0.05f));
            Button bb = new Button(new GLCoordinate(0.2f, 0), new GLCoordinate(0.05f, 0.05f));
            GUILine l = new GUILine(b, bb);

            Add(b);Add(l);

            this.AddOption();
            this.AddOption();
            this.AddOption();
            this.AddOption();
            this.AddOption();
        }

        public void AddOption()
        {
            // add button at irrelevant location
            float ARM_LENGTH = 0.2f;
            var BUTTON_SIZE = new GLCoordinate(0.05f, 0.05f);
            var button = new Button(new GLCoordinate(0, 0), BUTTON_SIZE);
            //OptionButton ob = new OptionButton(button, new Line(this, button));
            button.OnMouseButtonPress += (component, union) =>
            {
                Logging.Log("waaa");
            };
            this.Add(button);

            // position buttons in a circle
            int i = 1;
            foreach (var child in children)
            {
                double rads = (double)(2 * i * Math.PI / this.children.Count);
                float x = (float) Math.Cos(rads);
                float y = (float) Math.Sin(rads);

                i++;
                child.Location = new GLCoordinate( x * ARM_LENGTH, y * ARM_LENGTH);
            }
        }
    }

    class OptionButton : GUIComponent
    {
        private Button Button;
        private Line LineToParent;

        public OptionButton(Button button, Line lineToParent)
        {
            this.Button = button;
            this.LineToParent = lineToParent;
        }
    }
}
