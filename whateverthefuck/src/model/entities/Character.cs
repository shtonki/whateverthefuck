﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    public abstract class Character : GameEntity
    {
        public Character(EntityIdentifier identifier, EntityType entityType) : base(identifier, entityType)
        {
            Movable = true;
            Targetable = true;
            ShowHealth = true;
            Height = 1;
        }

        public override void Step(GameState gameState)
        {
            base.Step(gameState);
            if (CurrentHealth < 0)
            {
                gameState.HandleGameEvents(new DestroyEntityEvent(this));
            }
        }
    }
}
