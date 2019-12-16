namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using whateverthefuck.src.control;
    using whateverthefuck.src.model.entities;
    using whateverthefuck.src.network.messages;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;
    using whateverthefuck.src.view.guicomponents;

    /// <summary>
    /// Handles the client side Game State. Is responsible for handling incoming GameEvents and requesting GameEvents on the server side.
    /// </summary>
    internal class ClientGameStateManager
    {
        private const int TickInterval = 10;
        private const float OneOverSquareRootOfTwo = 0.70710678118f;

        public ClientGameStateManager()
        {
            this.GameState = new GameState();

            this.TickTimer = new Timer(_ => this.Tick(), null, 0, TickInterval);
        }

        public GameState GameState { get; private set; }

        private GUIComponent FocusedGUIComponent { get; set; }

        private Timer TickTimer { get; } // can't be removed or we stop moving after ~3 seconds

        private PlayerCharacter Hero { get; set; }

        private HeroMovementStruct HeroMovements { get; } = new HeroMovementStruct();

        private GameEntity TargetedEntity { get; set; }

        private List<GUIComponent> ClickedDownGuiComponents { get; set; } = new List<GUIComponent>();

        private float PrevDirection { get; set; }

        // fix
        private int? TakeControlId { get; set; }

        /// <summary>
        /// Updates the underlying GameState by handling the given events then stepping.
        /// </summary>
        /// <param name="tick">The servers tick counter.</param>
        /// <param name="events">The GameEvents to be handled.</param>
        public void UpdateGameState(int tick, IEnumerable<GameEvent> events)
        {
            if (tick == 0)
            {
                this.GameState.HandleGameEvents(events);
                return;
            }

            if (this.GameState.StepCounter <= 1)
            {
                this.GameState.StepCounter = tick;
            }

            this.GameState.HandleGameEvents(events);
            this.GameState.Step();
            this.UpdateLOS();

            var syncRecord = this.GameState.GenerateSyncRecord();

            if (syncRecord.Tick % 100 == 1)
            {
                Program.ServerConnection.SendMessage(new SyncMessage(syncRecord));
            }

            // fix me t. ribbe
            if (this.TakeControlId.HasValue)
            {
                this.Hero = (PlayerCharacter)this.GameState.GetEntityById(this.TakeControlId.Value);
                if (this.Hero != null)
                {
                    this.CenterCameraOn(this.Hero);
                }

                this.TakeControlId = null;
            }
        }

        /// <summary>
        /// Grants the player control of a GameEntity.
        /// </summary>
        /// <param name="identifier">The identifier of the GameEntity the player is to control.</param>
        public void TakeControl(int identifier)
        {
            this.TakeControlId = identifier;
        }

        public void CenterCameraOn(GameEntity entity)
        {
            GUI.Camera = new FollowCamera(this.Hero);
        }

        public void SpawnLoot(CreateLootMessage message)
          {
            var item = message.Item;
            var lootee = this.GameState.GetEntityById(message.LooteeId);

            Loot lootbox = new Loot(EntityIdentifier.RandomReserved(), new CreationArgs(0));
            lootbox.Center = lootee.Center;
            lootbox.Items.Add(item);
            var cevent = new CreateEntityEvent(lootbox);
            cevent.OnCreationCallback = e => this.AddLoot(e as Loot, item);
            this.GameState.HandleGameEvents(cevent);
        }

        public void HandleInput(InputUnion input)
        {
            GUIComponent interactedGUIComponent = null;

            if (input.Location != null)
            {
                interactedGUIComponent = this.FirstVisibleGUIComponentAt(input.Location.ToGLCoordinate());
            }

            if (input.IsMouseInput && input.Direction == InputUnion.Directions.Down)
            {
                this.Focus(interactedGUIComponent);
            }

            if (interactedGUIComponent != null && !input.IsKeyboardInput)
            {
                interactedGUIComponent.HandleInput(input);
            }
            else if (this.FocusedGUIComponent != null)
            {
                this.FocusedGUIComponent.HandleInput(input);
            }
            else
            {
                if (input.IsMouseInput)
                {
                    var v = this.GetEntityAtLocation(GUI.Camera.GLToGameCoordinate(input.Location));
                    if (v != null && v.Targetable)
                    {
                        this.Target(v);
                    }
                }
                else if (input.IsKeyboardInput)
                {
                    var action = Hotkeys.LookupHotkey(input);

                    if (action != GameAction.Undefined)
                    {
                        this.ActivateAction(action);
                    }
                }
            }
#if false
            if (input.IsMouseInput)
            {
                // todo handle visible component "underneath" invisible one
                var interacted = this.FirstVisibleGUIComponentAt(input.Location.ToGLCoordinate());

                if (input.Direction == InputUnion.Directions.Down)
                {
                    if (interacted != null && interacted.Visible)
                    {
                        this.Focus(interacted);
                    }
                    else
                    {
                        this.Focus(null);
                    }
                }
            }

            if (input.IsMouseMove)
            {
                // todo handle visible component "underneath" invisible one
                var interacted = this.FirstVisibleGUIComponentAt(input.Location.ToGLCoordinate());

                if (interacted != null)
                {
                    interacted.HandleInput(input);
                }
            }

            if (this.FocusedGUIComponent != null)
            {
                this.FocusedGUIComponent.HandleInput(input);
            }
            else
            {
            }
#endif
        }

        private void Target(GameEntity target)
        {
            if (this.TargetedEntity != null)
            {
                this.TargetedEntity.HighlightColor = Color.Transparent;
            }

            this.TargetedEntity = target;
            this.TargetedEntity.HighlightColor = Color.Coral;
        }

        private void Tick()
        {
            if (this.Hero != null)
            {
                MovementStruct ms = new MovementStruct();

                double direction = 0;

                if (this.HeroMovements.Upwards && this.HeroMovements.Leftwards)
                {
                    direction = 7 * Math.PI / 4;
                }
                else if (this.HeroMovements.Upwards && this.HeroMovements.Rightwards)
                {
                    direction = Math.PI / 4;
                }
                else if (this.HeroMovements.Downwards && this.HeroMovements.Leftwards)
                {
                    direction = 5 * Math.PI / 4;
                }
                else if (this.HeroMovements.Downwards && this.HeroMovements.Rightwards)
                {
                    direction = 3 * Math.PI / 4;
                }
                else if (this.HeroMovements.Leftwards)
                {
                    direction = 3 * Math.PI / 2;
                }
                else if (this.HeroMovements.Rightwards)
                {
                    direction = Math.PI / 2;
                }
                else if (this.HeroMovements.Upwards)
                {
                    direction = 2 * Math.PI;
                }
                else if (this.HeroMovements.Downwards)
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

                if (!ms.Direction.Equals(this.PrevDirection))
                {
                    var e = new UpdateMovementEvent(this.Hero.Identifier.Id, ms);
                    Program.ServerConnection.SendMessage(new UpdateGameStateMessage(0, e));
                }

                this.PrevDirection = ms.Direction;
            }
        }

        private void AddLoot(Loot e, Item item)
        {
            e.Items.Add(item);
        }

        private void Focus(GUIComponent focused)
        {
            this.FocusedGUIComponent = focused;
        }

        private void BeginCastAbility(Ability ability)
        {
            if (this.TargetedEntity == null)
            {
                return;
            }

            Program.ServerConnection.SendMessage(
                new UpdateGameStateMessage(0, new BeginCastAbilityEvent(this.Hero, this.TargetedEntity, ability)));
        }

        private void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.CastAbility1:
                {
                    this.BeginCastAbility(new Ability(AbilityType.Fireballx));
                } break;

                case GameAction.HeroWalkUpwards:
                {
                    this.HeroMovements.Upwards = true;
                } break;

                case GameAction.HeroWalkUpwardsStop:
                {
                    this.HeroMovements.Upwards = false;
                } break;

                case GameAction.HeroWalkDownwards:
                {
                    this.HeroMovements.Downwards = true;
                } break;

                case GameAction.HeroWalkDownwardsStop:
                {
                    this.HeroMovements.Downwards = false;
                } break;

                case GameAction.HeroWalkLeftwards:
                {
                    this.HeroMovements.Leftwards = true;
                } break;

                case GameAction.HeroWalkLeftwardsStop:
                {
                    this.HeroMovements.Leftwards = false;
                } break;

                case GameAction.HeroWalkRightwards:
                {
                    this.HeroMovements.Rightwards = true;
                } break;

                case GameAction.HeroWalkRightwardsStop:
                {
                    this.HeroMovements.Rightwards = false;
                } break;

                case GameAction.CameraZoomIn:
                {
                    GUI.Camera?.Zoom.ZoomIn();
                } break;

                case GameAction.CameraZoomOut:
                {
                    GUI.Camera?.Zoom.ZoomOut();
                } break;

                case GameAction.TogglePanel:
                {
                    foreach (var panel in GUI.GUIComponents.Where(p => p is Panel))
                    {
                        panel.Visible = !panel.Visible;
                    }
                } break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }

        private void UpdateLOS()
        {
            var inLOS = LineOfSight.CheckLOS(this.Hero, this.GameState.AllEntities);

            foreach (var e in inLOS)
            {
                e.LOSGraceTicks = 5;
            }

            foreach (var e in this.GameState.AllEntities)
            {
                e.LOSGraceTicks--;
                e.Visible = e.LOSGraceTicks > 0;
            }
        }

        private GameEntity GetEntityAtLocation(GameCoordinate location)
        {
            var mp = new MousePicker();
            mp.Center = location;
            var picked = this.GameState.Intersects(mp);
            if (picked.Count() == 0) { return null; }
            return picked.First();
        }

        private GUIComponent FirstVisibleGUIComponentAt(GLCoordinate location)
        {
            foreach (var c in GUI.GUIComponents)
            {
                if (c.Contains(location) && c.Visible)
                {
                    return c;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// A container for the movement info of the Hero.
    /// </summary>
    internal class HeroMovementStruct
    {
        public bool Upwards { get; set; }

        public bool Downwards { get; set; }

        public bool Rightwards { get; set; }

        public bool Leftwards { get; set; }
    }
}
