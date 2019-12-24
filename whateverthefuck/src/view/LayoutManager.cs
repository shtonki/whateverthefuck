using whateverthefuck.src.view.guicomponents;

namespace whateverthefuck.src.view
{
    public abstract class LayoutManager
    {
        public abstract void Layout(GUIComponent parent, GUIComponent child);
    }

    public class GridLayoutManager : LayoutManager
    {
        public int Columns { get; set; }

        public int Rows { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float XPadding { get; set; }

        public float YPadding { get; set; }

        // this piece of shit is probably broken if you use it for anything other
        // than what i specifically needed it to do when i made it.
        public override void Layout(GUIComponent parent, GUIComponent child)
        {
            int c = parent.Children.Count;
            int column = 0;
            int row = 0;

            if (this.Rows != 0)
            {
                column = c / this.Rows;
                row = c % this.Rows;
            }

            float x = this.XPadding + ((this.Width + this.XPadding) * column);
            float y = this.YPadding + ((this.Height + this.YPadding) * row);

            child.Size = new GLCoordinate(this.Width, this.Height);
            child.Location = new GLCoordinate(x, y);
        }
    }
}
