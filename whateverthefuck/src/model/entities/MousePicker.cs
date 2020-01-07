﻿namespace whateverthefuck.src.model.entities
{
    using System;
    using System.Drawing;
    using whateverthefuck.src.view;

    internal class MousePicker : GameEntity
    {
        public MousePicker()
            : base(EntityIdentifier.Invalid, EntityType.GameMechanic, new CreationArgs(0))
        {
            this.DrawColor = Color.CornflowerBlue;
            this.Size = new GameCoordinate(0.000001f, 0.000001f);
            throw new NotImplementedException();
        }

    }
}
