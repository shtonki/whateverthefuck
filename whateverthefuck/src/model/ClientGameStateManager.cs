using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
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

        private HeroMovementStruct HeroMovements = new HeroMovementStruct();

        private GameEntity Target { get; set; }


        public ClientGameStateManager()
        {
            GameState = new GameState();

            TickTimer = new Timer(_ => Tick(), null, 0, TickInterval);
        }

        const int div = 4;
        const int mul = div - 1;

        private float PrevDirection;

        private const float OneOverSquareRootOfTwo = 0.70710678118f;


        private void Tick()
        {
#if false
            if (UseSmoothing)
            {
                SmoothMovements();
            }
#endif
            if (Hero != null)
            {
                MovementStruct ms = new MovementStruct();

                double direction = 0;

                if (HeroMovements.Upwards && HeroMovements.Leftwards)
                {
                    direction = 7 * Math.PI / 4;
                }
                else if (HeroMovements.Upwards && HeroMovements.Rightwards)
                {
                    direction = Math.PI / 4;
                }
                else if (HeroMovements.Downwards && HeroMovements.Leftwards)
                {
                    direction = 5 * Math.PI / 4;
                }
                else if (HeroMovements.Downwards && HeroMovements.Rightwards)
                {
                    direction = 3 * Math.PI / 4;
                }
                else if (HeroMovements.Leftwards)
                {
                    direction = 3*Math.PI/2;
                }
                else if (HeroMovements.Rightwards)
                {
                    direction = Math.PI/2;
                }
                else if (HeroMovements.Upwards)
                {
                    direction = 2*Math.PI;
                }
                else if (HeroMovements.Downwards)
                {
                    direction = Math.PI;
                }


                if (direction == 0)
                {
                    ms.Direction = float.NaN;
                }
                else
                {
                    ms.Direction = (float)direction;
                }


                if (!ms.Direction.Equals(PrevDirection))
                {
                    var e = new UpdateMovementEvent(Hero.Identifier.Id, ms);
                    Program.ServerConnection.SendMessage(new UpdateGameStateMessage(e));
                }

                PrevDirection = ms.Direction;
            }
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
                e.Visible = e.LOSGraceTicks > 0;
            }
        }

        private int? TakeControlId;

        public void TakeControl(int identifier)
        {
            TakeControlId = identifier;
        }

        public void CenterCameraOn(GameEntity entity)
        {
            GUI.Camera = new FollowCamera(Hero);
        }

        public void UpdateGameState(IEnumerable<GameEvent> events)
        {
            GameState.HandleGameEvents(events);
            GameState.Step();
            UpdateLOS();

            if (TakeControlId.HasValue)
            {
                Hero = (PlayerCharacter)GameState.GetEntityById(TakeControlId.Value);
                if (Hero != null)
                {
                    CenterCameraOn(Hero);
                }
                TakeControlId = null;
            }
#if false
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
#endif
        }

        public void HandleGUIClick(MouseButtonEventArgs me, GLCoordinate clicked)
        {
            foreach (var guiComponent in GUI.GUIComponents)
            {
                if (guiComponent.Contains(clicked))
                {
                    if (me.Button == MouseButton.Left && me.IsPressed)
                    {
                        guiComponent.OnLeftMouseDown();
                    }
                    else if (me.Button == MouseButton.Left && !me.IsPressed)
                    {
                        guiComponent.OnLeftMouseUp();
                    }

                    if (me.Button == MouseButton.Right && me.IsPressed)
                    {
                        guiComponent.OnRightMouseDown();
                    }
                    else if (me.Button == MouseButton.Right && !me.IsPressed)
                    {
                        guiComponent.OnRightMouseUp();
                    }
                }
            }
        }

        private GameEntity GetEntityAtLocation(GameCoordinate location)
        {
            var mp = new MousePicker();
            mp.Center = location;
            var picked = GameState.Intersects(mp);
            if (picked.Count() == 0) { return null; }
            return picked.First();
        }


        public void HandleWorldClick(MouseButtonEventArgs me, GameCoordinate clickLocation)
        {
            if (me.Button == MouseButton.Left && me.IsPressed)
            {
                var ge = GetEntityAtLocation(clickLocation);
                if (ge != null && ge.Targetable)
                {
                    if (Target != null)
                    {
                        Target.HighlightColor = Color.Transparent;
                    }

                    Target = ge;
                    Target.HighlightColor = Color.DarkOrange;
                }
            }
            else if (me.Button == MouseButton.Left && !me.IsPressed)
            {

            }

            if (me.Button == MouseButton.Right && me.IsPressed)
            {

            }
            else if (me.Button == MouseButton.Right && !me.IsPressed)
            {

            }
        }

        public void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.HeroWalkUpwards:
                {
                    HeroMovements.Upwards = (true);
                } break;

                case GameAction.HeroWalkUpwardsStop:
                {
                    HeroMovements.Upwards = (false);
                } break;

                case GameAction.HeroWalkDownwards:
                {
                    HeroMovements.Downwards = (true);
                } break;

                case GameAction.HeroWalkDownwardsStop:
                {
                    HeroMovements.Downwards = (false);
                } break;

                case GameAction.HeroWalkLeftwards:
                {
                    HeroMovements.Leftwards = (true);
                } break;

                case GameAction.HeroWalkLeftwardsStop:
                {
                    HeroMovements.Leftwards = (false);
                } break;

                case GameAction.HeroWalkRightwards:
                {
                    HeroMovements.Rightwards = (true);
                } break;

                case GameAction.HeroWalkRightwardsStop:
                {
                    HeroMovements.Rightwards = (false);
                } break;

                case GameAction.CameraZoomIn:
                    {
                        GUI.Camera?.Zoom.ZoomIn();
                    }
                    break;

                case GameAction.CameraZoomOut:
                    {
                        GUI.Camera?.Zoom.ZoomOut();
                    } break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }

    }


    public class HeroMovementStruct
    {
        public bool Upwards { get; set; }
        public bool Downwards { get; set; }
        public bool Rightwards { get; set; }
        public bool Leftwards { get; set; }
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
