namespace whateverthefuck.src.view.guicomponents
{
    using System.Drawing;
    using whateverthefuck.src.util;

    internal class Button : GUIComponent
    {
        public Button()
            : base()
        {
            // fix t ribbe
            this.OnMouseButtonPress += (c, i) =>
            {
                if (i.Direction == control.InputUnion.Directions.Down)
                {
                    Logging.Log("down");
                }
                else if (i.Direction == control.InputUnion.Directions.Up)
                {
                    Logging.Log("up");
                }
            };
        }

        public Button(GLCoordinate location, GLCoordinate size)
            : base(location, size)
        {
            this.BackColor = Color.DarkGoldenrod;

            this.OnMouseButtonPress += (c, i) =>
            {
                if (i.Direction == control.InputUnion.Directions.Down)
                {
                    Logging.Log("down");
                }
                else if (i.Direction == control.InputUnion.Directions.Up)
                {
                    Logging.Log("up");
                }
            };

#if false
            OnLeftMouseDown += (a) =>
            {
                Logging.Log(this.GetType() + " was Left Pressed.");
            };
            OnLeftMouseUp += (a) =>
            {
                Logging.Log(this.GetType() + " was Left Released.");
            };
            OnRightMouseDown += (a) =>
            {
                Logging.Log(this.GetType() + " was Right Pressed.");
            };
            OnRightMouseUp += (a) =>
            {
                Logging.Log(this.GetType() + " was Right Released.");
            };
#endif
        }
    }
}
