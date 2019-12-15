namespace whateverthefuck.src.view
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view.guicomponents;

    class SlickInteractionMenu : GUIComponent
    {
        private int nrOfOptions = 0;

        public SlickInteractionMenu(GLCoordinate location, GLCoordinate size) : base(location, size)
        {
            AddOption();
        }

        private void AddOption()
        {
            nrOfOptions++;
            double rads =  (2*Math.PI) / (nrOfOptions);
            Logging.Log(rads.ToString());

            //base.Add(toAdd);
        }
    }
}
