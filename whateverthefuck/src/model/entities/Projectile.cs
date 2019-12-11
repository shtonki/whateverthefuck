using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    class Projectile : GameEntity
    {
        private float AsplodeCutoff => MoveSpeed * 2;

        public Projectile(EntityIdentifier id) : base(id, EntityType.Projectile)
        {
            Size = new GameCoordinate(0.01f, 0.01f);
            DrawColor = Color.Black;
            MoveSpeed = 0.02f;
            Collidable = false;
        }

        public override void Step(GameState gameState)
        {
            base.Step(gameState);

            if (Movements.IsFollowing)
            {
                var followed = gameState.GetEntityById(Movements.FollowId.Value);
                if (DistanceTo(followed.Center) < AsplodeCutoff)
                {
                    gameState.HandleGameEvents(new DestroyEntityEvent(this));
                    followed.CurrentHealth -= 10;
                }
            }
        }
    }
}
