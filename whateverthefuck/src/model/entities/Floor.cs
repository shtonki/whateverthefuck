﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    class Floor : GameEntity
    {
        public Floor(EntityIdentifier id) : base(id, EntityType.Floor)
        {
            Collidable = false;
            BlocksLOS = false;
            DrawColor = Color.AntiqueWhite;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            base.DrawMe(drawAdapter);
        }
    }
}