namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.util;

    internal class Button : GUIComponent
    {
        public Button()
            : base()
        {
        }

        public Button(GLCoordinate location, GLCoordinate size)
            : base(location, size)
        {
            this.BackColor = Color.DarkGoldenrod;
        }
    }
}
