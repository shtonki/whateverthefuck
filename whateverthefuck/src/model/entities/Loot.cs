﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model.entities
{
    class Loot : GameEntity
    {
        public Loot(EntityIdentifier identifier, CreationArgs args) : base(identifier, EntityType.Loot, args)
        {
            DrawColor = Color.Gold;
            Size = new GameCoordinate(0.05f, 0.05f);
            Collidable = false;
        }
    }
}