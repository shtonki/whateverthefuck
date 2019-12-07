using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using whateverthefuck.src.control;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class ClientGameStateManager
    {
        private Timer TickTimer;

        private PlayerCharacter Hero;

        public GameState GameState { get; set; }

        private Dictionary<int, SpicyClass> SpicyDictionary = new Dictionary<int, SpicyClass>();
        private bool UseSmoothing = true;

        private const int TickInterval = 10;

        public ClientGameStateManager()
        {
            GameState = new GameState();

            TickTimer = new Timer(_ => Tick(), null, 0, TickInterval);
        }

        const int div = 4;
        const int mul = div - 1;

        private void Tick()
        {
            if (UseSmoothing)
            {
                SmoothMovements();
            }

            if (Hero == null)
            {
                return;
            }

            Program.ServerConnection.SendMessage(new UpdatePlayerControlMessage(Hero));

            UpdateLOS();
        }


        private void SmoothMovements()
        {
            foreach (var entity in GameState.AllEntities)
            {
                if (SpicyDictionary.ContainsKey(entity.Identifier.Id))
                {
                    var spicyEntry = SpicyDictionary[entity.Identifier.Id];

                    var locX = mul * entity.Location.X / div + spicyEntry.NewLocation.X / div;
                    var locY = mul * entity.Location.Y / div + spicyEntry.NewLocation.Y / div;

                    var loc = new GameCoordinate(locX, locY);
                    var diff = loc - (GameCoordinate)entity.Location;

                    var newSpeedX = spicyEntry.Speed.X / 2 + diff.X / 2;
                    var newSpeedY = spicyEntry.Speed.Y / 2 + diff.Y / 2;

                    var newSpeed = new GameCoordinate(newSpeedX, newSpeedY);

                    entity.Location = newSpeed + (GameCoordinate)entity.Location;
                    spicyEntry.Speed = newSpeed;
                }
            }
        }

        public class Rectangle : Drawable
        {
            public float X1;
            public float Y1;
            public float X2;
            public float Y2;

            public Rectangle(GameEntity o) : base(new GameCoordinate(0, 0))
            {
                X1 = o.Left;
                Y1 = o.Bottom;
                X2 = o.Right;
                Y2 = o.Top;
            }

            public override void DrawMe(DrawAdapter drawAdapter)
            {
                drawAdapter.FillLine(X1, Y1, X2, Y1, Color.White);
                drawAdapter.FillLine(X2, Y1, X2, Y2, Color.White);
                drawAdapter.FillLine(X2, Y2, X1, Y2, Color.White);
                drawAdapter.FillLine(X1, Y2, X1, Y1, Color.White);
            }
        }

        

        private void UpdateLOS()
        {
            var inLOS = LineOfSight.CheckLOS(Hero, GameState.AllEntities);

            foreach (var e in inLOS)
            {
                e.LOSGraceTicks = 5;
            }

            foreach (var e in GameState.AllEntities)
            {
                e.LOSGraceTicks--;
                e.Invisible = e.LOSGraceTicks < 0;
            }
        }

        public void CreateEntity(CreateEntityInfo info)
        {
            var entity = GameState.EntityGenerator.GenerateEntity((EntityType)info.EntityType);
            entity.Identifier = new EntityIdentifier(info.Identifier);
            entity.Location = new GameCoordinate(info.X, info.Y);
            GameState.AddEntity(entity);
        }

        public void RemoveEntity(EntityIdentifier id)
        {
            GameState.RemoveEntity(id);
        }

        public void TakeControl(int identifier)
        {
            Hero = (PlayerCharacter)GameState.GetEntityById(identifier);
            CenterCameraOn(Hero);
        }

        public void CenterCameraOn(GameEntity entity)
        {
            GUI.Camera = new FollowCamera(Hero);
        }

        private Semaphore UpdateLocationSemaphore = new Semaphore(1, 1);

        public void UpdateLocations(IEnumerable<EntityLocationInfo> infos)
        {
            if (UpdateLocationSemaphore.WaitOne(new TimeSpan(1)))
            {
                var newTick = DateTime.UtcNow;

                foreach (var info in infos)
                {
                    var updatee = GameState.GetEntityById(info.Identifier);

                    if (updatee == null)
                    {
                        Logging.Log("Got position of Entity we don't think exists.", Logging.LoggingLevel.Warning);
                    }
                    else if (UseSmoothing)
                    {

                        if (!SpicyDictionary.ContainsKey(updatee.Identifier.Id))
                        {
                            var spicy = new SpicyClass(updatee.Identifier.Id);
                            SpicyDictionary.Add(updatee.Identifier.Id, spicy);
                        }

                        var newLocation = new GameCoordinate(info.X, info.Y);
                        var spicyEntry = SpicyDictionary[updatee.Identifier.Id].NewLocation = newLocation;
                    }
                    else
                    {
                        updatee.Location = new GameCoordinate(info.X, info.Y);
                    }
                }
                UpdateLocationSemaphore.Release();
            }
            else
            {
        }
    }

        public void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.HeroWalkUpwards:
                    {
                        Hero?.SetMovementUpwards(true);
                    }
                    break;

                case GameAction.HeroWalkUpwardsStop:
                    {
                        Hero?.SetMovementUpwards(false);
                    }
                    break;

                case GameAction.HeroWalkDownwards:
                    {
                        Hero?.SetMovementDownwards(true);
                    }
                    break;

                case GameAction.HeroWalkDownwardsStop:
                    {
                        Hero?.SetMovementDownwards(false);
                    }
                    break;

                case GameAction.HeroWalkLeftwards:
                    {
                        Hero?.SetMovementLeftwards(true);
                    }
                    break;

                case GameAction.HeroWalkLeftwardsStop:
                    {
                        Hero?.SetMovementLeftwards(false);
                    }
                    break;

                case GameAction.HeroWalkRightwards:
                    {
                        Hero?.SetMovementRightwards(true);
                    }
                    break;

                case GameAction.HeroWalkRightwardsStop:
                    {
                        Hero?.SetMovementRightwards(false);
                    }
                    break;

                case GameAction.CameraZoomIn:
                    {
                        GUI.Camera?.Zoom.ZoomIn();
                    }
                    break;

                case GameAction.CameraZoomOut:
                    {
                        GUI.Camera?.Zoom.ZoomOut();
                    }
                    break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }

    }

    class SpicyClass
    {
        public int Id { get; }
        public GameCoordinate NewLocation { get; set; }
        public GameCoordinate Speed { get; set; }

        public SpicyClass(int id)
        {
            Id = id;
            Speed = new GameCoordinate(0, 0);
        }
    }
}
