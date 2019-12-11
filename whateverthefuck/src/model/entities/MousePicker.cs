using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    class MousePicker : GameEntity
    {
        public MousePicker() : base(EntityIdentifier.Invalid, EntityType.GameMechanic, new CreationArgs(0))
        {
            Visible = true;
            DrawColor = Color.CornflowerBlue;
            Size = new GameCoordinate(0.000001f, 0.000001f);
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {

        }
    }
}
