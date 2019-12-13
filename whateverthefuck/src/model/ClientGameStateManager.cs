using System;
using System.Collections;
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

        private List<GUIComponent> ClickedDownGuiComponents = new List<GUIComponent>();

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
#if false
        public void HandleMouseScroll(InputUnion mouseInput, ScreenCoordinate screenMouseLocation)
        {
            var gl = GUI.TranslateScreenToGLCoordinates(screenMouseLocation);

            var scrolledComponents = GUI.GUIComponents.Where(g => g.Visible && g.Contains(gl));

            foreach (var scrolledComponent in scrolledComponents)
            {
                scrolledComponent.HandleScroll(mouseInput, gl);
            }

            if (scrolledComponents.Any()) return;

            Input.Handle(mouseInput);
        }
#endif
        public void SpawnLoot(CreateLootMessage message)
          {
            var item = message.Item;
            var lootee = GameState.GetEntityById(message.LooteeId);

            Loot lootbox = new Loot(EntityIdentifier.RandomReserved(), CreationArgs.Zero);
            lootbox.Location = lootee.Location;
            lootbox.Items.Add(item);
            var cevent = new CreateEntityEvent(lootbox);
            cevent.OnCreationCallback = e => AddLoot(e as Loot, item);
            GameState.HandleGameEvents(cevent);

            int i = 4;
        }

        private void AddLoot(Loot e, Item item)
        {
            e.Items.Add(item);
        }
#if false
        public void HandleMouseMove(ScreenCoordinate screenClickLocation)
        {
            var gl = GUI.TranslateScreenToGLCoordinates(screenClickLocation);
            foreach (var guiComponent in GUI.GUIComponents)
            {
                guiComponent.HandleMouseMove(gl);
            }
        }
 
        private bool HandleGuiClick(InputUnion input, GLCoordinate location)
        {
            var clicked = GUI.GUIComponents.Where(g => g.Visible && g.Contains(location));

            if (clicked.Count() == 0)
            {
                if (input.Direction == InputUnion.Directions.Up && ClickedDownGuiComponents.Count() > 0)
                {
                    ClickedDownGuiComponents.ForEach(cgc => cgc.HandleClick(input, location));
                    ClickedDownGuiComponents.Clear();
                }

                return false;
            }

            if (input.Direction == InputUnion.Directions.Down)
            {
                ClickedDownGuiComponents = clicked.ToList();
                ClickedDownGuiComponents.ForEach(cgc => cgc.HandleClick(input, location));
            }
            else if (input.Direction == InputUnion.Directions.Up)
            {
                ClickedDownGuiComponents.ForEach(cgc => cgc.HandleClick(input, location));
                ClickedDownGuiComponents.Clear();
            }

            return true;
        }

        private bool HandleGameClick(InputUnion input, GameCoordinate location)
        {
            Logging.Log(location.ToString());
            var clickedEntity = GetEntityAtLocation(location);
            if (clickedEntity == null) { return false; }

            //clickedEntity.Interact(input);

            if (input.MouseButton == MouseButton.Left && input.Direction == InputUnion.Directions.Down)
            {
                if (clickedEntity.Targetable)
                {
                    if (Target != null)
                    {
                        Target.HighlightColor = Color.Transparent;
                    }

                    Target = clickedEntity;
                    Target.HighlightColor = Color.DarkOrange;
                }
            }
            return true;
        }
#endif
        private GUIComponent Focused;

        private void Focus(GUIComponent focused)
        {
            Focused = focused;
        }

        public void HandleInput(InputUnion input)
        {
            if (input.IsMouseInput && input.Direction == InputUnion.Directions.Down)
            {
                var clicked = GUIComponentAt(input.Location.ToGLCoordinate());

                if (clicked != null)
                {
                    Focus(clicked);
                }
                else
                {
                    Focus(null);
                }
            }

            if (input.IsMouseInput && input.Direction == InputUnion.Directions.Up && Focused == null)
            {
                Focus(null);
            }

            if (Focused != null)
            {
                Focused.HandleInput(input);
            }
            else
            {
                if (input.IsMouseInput)
                {
                    var v = GetEntityAtLocation(GUI.Camera.GLToGameCoordinate(input.Location));
                    if (v != null)
                    {
                        Logging.Log(v);
                    }
                }
                else if (input.IsKeyboardInput)
                {
                    var action = Input.LookupHotkey(input);

                    if (action != GameAction.Undefined)
                    {
                        ActivateAction(action);
                    }
                }
            }
        }
#if false
        public void HandleClick(InputUnion mouseInput)
        {
            var screenClickLocation = mouseInput.Location;
            var glLocation = GUI.TranslateScreenToGLCoordinates(screenClickLocation);
            var gameLocation = GUI.Camera.GLToGameCoordinate(glLocation);

            if (HandleGuiClick(mouseInput, glLocation))
            {
                return;
            }
            else
            {
                HandleGameClick(mouseInput, gameLocation);
            }
        }
#endif
        private void ActivateAction(GameAction gameAction)
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

        private GUIComponent GUIComponentAt(GLCoordinate location)
        {
            foreach (var c in GUI.GUIComponents)
            {
                if (c.Contains(location))
                {
                    return c;
                }
            }
            return null;
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
