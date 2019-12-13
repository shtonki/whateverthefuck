namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientGameStateManager"/> class.
        /// </summary>
        public ClientGameStateManager()
        {
            this.GameState = new GameState();

            new Timer(_ => this.Tick(), null, 0, TickInterval);
        }

        /// <summary>
        /// Gets the GameState representing the clients state of the game.
        /// </summary>
        public GameState GameState { get; private set; }

        private PlayerCharacter Hero { get; set; }

        private HeroMovementStruct HeroMovements { get; } = new HeroMovementStruct();

        private GameEntity Target { get; set; }

        private List<GUIComponent> ClickedDownGuiComponents { get; set; } = new List<GUIComponent>();

        private float PrevDirection { get; set; }

        // fix
        private int? TakeControlId { get; set; }

        /// <summary>
        /// Updated the underlying GameState by handling the given events then stepping.
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

        /// <summary>
        /// Centers the Camera on a GameEntity.
        /// </summary>
        /// <param name="entity">The GameEntity to center the camera on.</param>
        public void CenterCameraOn(GameEntity entity)
        {
            GUI.Camera = new FollowCamera(this.Hero);
        }

        /// <summary>
        /// Spawns loot for the player.
        /// </summary>
        /// <param name="message">The message containing informaion about what loot to spawn.</param>
        public void SpawnLoot(CreateLootMessage message)
          {
            var item = message.Item;
            var lootee = this.GameState.GetEntityById(message.LooteeId);

            Loot lootbox = new Loot(EntityIdentifier.RandomReserved(), new CreationArgs(0));
            lootbox.Location = lootee.Location;
            lootbox.Items.Add(item);
            var cevent = new CreateEntityEvent(lootbox);
            cevent.OnCreationCallback = e => this.AddLoot(e as Loot, item);
            this.GameState.HandleGameEvents(cevent);
        }

        /// <summary>
        /// Handles user input into the game.
        /// </summary>
        /// <param name="input">The input to be handled.</param>
        public void HandleInput(InputUnion input)
        {
            if (input.IsMouseInput && input.Direction == InputUnion.Directions.Down)
            {
                var clicked = this.GUIComponentAt(input.Location.ToGLCoordinate());

                if (clicked != null)
                {
                    this.Focus(clicked);
                }
                else
                {
                    this.Focus(null);
                }
            }

            if (input.IsMouseInput && input.Direction == InputUnion.Directions.Up && this.Focused == null)
            {
                this.Focus(null);
            }

            if (this.Focused != null)
            {
                this.Focused.HandleInput(input);
            }
            else
            {
                if (input.IsMouseInput)
                {
                    var v = this.GetEntityAtLocation(GUI.Camera.GLToGameCoordinate(input.Location));
                    if (v != null)
                    {
                        Logging.Log(v);
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

        private GUIComponent Focused { get; set; }

        private void Focus(GUIComponent focused)
        {
            this.Focused = focused;
        }

        private void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.CastAbility1:
                {
                    if (this.GameState.GetEntityById(this.Target.Identifier.Id) != null)
                    {
                        Program.ServerConnection.SendMessage(
                            new UpdateGameStateMessage(0, new UseAbilityEvent(this.Hero, this.Target, new Ability(Abilities.Fireballx))));
                    }
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

    /// <summary>
    /// A container for the movement info of the Hero.
    /// </summary>
    internal class HeroMovementStruct
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Hero is moving upwards.
        /// </summary>
        public bool Upwards { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Hero is moving downwards.
        /// </summary>
        public bool Downwards { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Hero is moving rightwards.
        /// </summary>
        public bool Rightwards { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Hero is moving leftwards.
        /// </summary>
        public bool Leftwards { get; set; }
    }
}
