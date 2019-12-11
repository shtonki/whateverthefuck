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
using whateverthefuck.src.view.guicomponents;

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
                    Program.ServerConnection.SendMessage(new UpdateGameStateMessage(0, e));
                }

                PrevDirection = ms.Direction;
            }
        }

        public void UpdateGameState(int tick, IEnumerable<GameEvent> events)
        {
            if (tick == 0)
            {
                GameState.HandleGameEvents(events);
                //GameState.Step();
                return;
            }

            if (GameState.StepCounter <= 1)
            {
                GameState.StepCounter = tick;
            }


            GameState.HandleGameEvents(events);
            GameState.Step();
            UpdateLOS();

            var syncRecord = GameState.GenerateSyncRecord();

            if (syncRecord.Tick % 100 == 1)
            {
                Program.ServerConnection.SendMessage(new SyncMessage(syncRecord));
            }

            // fix me t. ribbe
            if (TakeControlId.HasValue)
            {
                Hero = (PlayerCharacter)GameState.GetEntityById(TakeControlId.Value);
                if (Hero != null)
                {
                    CenterCameraOn(Hero);
                }
                TakeControlId = null;
            }
        }

        public void TakeControl(int identifier)
        {
            TakeControlId = identifier;
        }

        public void CenterCameraOn(GameEntity entity)
        {
            GUI.Camera = new FollowCamera(Hero);
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


        public void HandleMouseMove(ScreenCoordinate screenClickLocation)
        {
            var gl = GUI.TranslateScreenToGLCoordinates(screenClickLocation);
            foreach (var guiComponent in GUI.GUIComponents)
            {
                guiComponent.HandleMouseMove(gl);
            }
        }

        public void HandleClick(InputUnion mouseInput, ScreenCoordinate screenClickLocation)
        {
            var gl = GUI.TranslateScreenToGLCoordinates(screenClickLocation);
            var gc = GUI.Camera.GLToGameCoordinate(gl);

            var clickedGuiComponents = GUI.GUIComponents.Where(g => g.Visible && g.Contains(gl));

            foreach (var cgc in clickedGuiComponents)
            {
                cgc.HandleClick(mouseInput, gl);
            }


            if (clickedGuiComponents.Any()) return;

            if (mouseInput.MouseButton == MouseButton.Left && mouseInput.Direction == InputUnion.Directions.Down)
            {
                var ge = GetEntityAtLocation(gc);
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
        }

        public void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.CastAbility1:
                    {
                        if (GameState.GetEntityById(Target.Identifier.Id) != null)
                        {
                            Program.ServerConnection.SendMessage(
                                new UpdateGameStateMessage(0, new UseAbilityEvent(Hero, Target, new Ability(Abilities.Fireballx))));
                        }
                    }
                    break;

                case GameAction.HeroWalkUpwards:
                    {
                        HeroMovements.Upwards = (true);
                    }
                    break;

                case GameAction.HeroWalkUpwardsStop:
                    {
                        HeroMovements.Upwards = (false);
                    }
                    break;

                case GameAction.HeroWalkDownwards:
                    {
                        HeroMovements.Downwards = (true);
                    }
                    break;

                case GameAction.HeroWalkDownwardsStop:
                    {
                        HeroMovements.Downwards = (false);
                    }
                    break;

                case GameAction.HeroWalkLeftwards:
                    {
                        HeroMovements.Leftwards = (true);
                    }
                    break;

                case GameAction.HeroWalkLeftwardsStop:
                    {
                        HeroMovements.Leftwards = (false);
                    }
                    break;

                case GameAction.HeroWalkRightwards:
                    {
                        HeroMovements.Rightwards = (true);
                    }
                    break;

                case GameAction.HeroWalkRightwardsStop:
                    {
                        HeroMovements.Rightwards = (false);
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
                case GameAction.TogglePanel:
                    {
                        foreach (var panel in GUI.GUIComponents.Where(p => p is Panel))
                        {
                            panel.Visible = !panel.Visible;
                        }
                    }
                    break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
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

        private GameEntity GetEntityAtLocation(GameCoordinate location)
        {
            var mp = new MousePicker();
            mp.Center = location;
            var picked = GameState.Intersects(mp);
            if (picked.Count() == 0) { return null; }
            return picked.First();
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
