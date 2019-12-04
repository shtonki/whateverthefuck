﻿using System.Drawing;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class PlayerCharacter : Character
    {
        public PlayerCharacter(ControlInfo controlInfo, EntityIdentifier identifier) : base(controlInfo, identifier)
        {
            DrawColor = Coloring.RandomColor();
        }
    }
}