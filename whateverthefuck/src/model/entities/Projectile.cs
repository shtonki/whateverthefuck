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

        private int controller;

        public Projectile(EntityIdentifier id, CreationArgs args) : base(id, EntityType.Projectile, args)
        {
            if (args.Value != 0)
            {
                controller = new ProjectileArgs(args).ControllerId;
            }

            Size = new GameCoordinate(0.01f, 0.01f);
            DrawColor = Color.Black;
            MoveSpeed = 0.02f;
            Collidable = false;
        }

        private IEnumerable<GameEvent> Boom(GameEntity followed)
        {
            return new GameEvent[] { new DealDamageEvent(controller, followed.Identifier.Id, 10), new DestroyEntityEvent(this) };
        }

        private IEnumerable<GameEvent> Fizzle()
        {
            return new GameEvent[] { new DestroyEntityEvent(this) };
        }

        public override void Step(GameState gameState)
        {
            base.Step(gameState);

            if (Movements.IsFollowing)
            {
                var followed = gameState.GetEntityById(Movements.FollowId.Value);

                if (followed == null)
                {
                    gameState.HandleGameEvents(Fizzle());
                }
                else if (DistanceTo(followed.Center) < AsplodeCutoff)
                {
                    gameState.HandleGameEvents(Boom(followed));
                }
            }
        }
    }

    public class ProjectileArgs : CreationArgs
    {
        public int ControllerId
        {
            get { return SecondInt; }
            set { SecondInt = value; }
        }


        public ProjectileArgs(GameEntity c) : base(0)
        {
            ControllerId = c.Identifier.Id;
        }
        public ProjectileArgs(CreationArgs a) : base(a.Value)
        {

        }
    }
}
