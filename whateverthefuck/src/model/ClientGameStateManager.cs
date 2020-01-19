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
        private const float LootingMaxDistance = 0.25f;

        public ClientGameStateManager()
        {
            this.GameState = new GameState();

            this.TickTimer = new Timer(_ => this.Tick(), null, 0, TickInterval);

            this.Inventory.OnInventoryChanged += () =>
            {
                GUI.UpdateInventoryPanel(this.Inventory);
            };
        }

        public GameState GameState { get; private set; }

        public PC Hero { get; private set; }

        private Timer TickTimer { get; } // can't be removed or we stop moving after ~3 seconds

        private Lootable Looting { get; set; }

        private HeroMovementStruct HeroMovements { get; } = new HeroMovementStruct();

        public Inventory Inventory { get; private set; } = new Inventory();

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

            if (this.GameState.StepCounter == 1)
            {
                this.GameState.StepCounter = tick;
            }

            this.GameState.HandleGameEvents(events);
            this.GameState.Step();
            this.UpdateLOS();

            if (tick % 100 == 0)
            {
                var syncRecord = this.GameState.GenerateSyncRecord();
                Program.ServerConnection.SendMessage(new SyncMessage(syncRecord));
            }

            // @fix me t. ribbe
            if (this.TakeControlId.HasValue)
            {
                this.Hero = (PC)this.GameState.GetEntityById(this.TakeControlId.Value);
                if (this.Hero != null)
                {
                    this.CenterCameraOn(this.Hero);
                    GUI.LoadHUD(this.Hero);
                }

                this.TakeControlId = null;
            }
        }

        /// <summary>
        /// Grants the player control of a GameEntity.
        /// </summary>
        /// <param name="identifier">The identifier of the GameEntity the player is to control.</param>
        public void TakeControl(EntityIdentifier identifier)
        {
            this.TakeControlId = identifier.Id;
        }

        public void CenterCameraOn(GameEntity entity)
        {
            this.GameState.CurrentCamera = new FollowCamera(this.Hero);
        }

        public void SpawnLoot(EntityIdentifier looteeIdentifier, IEnumerable<Item> items)
          {
            var lootee = this.GameState.GetEntityById(looteeIdentifier) as Lootable;

            if (lootee != null)
            {
                foreach (var item in items)
                {
                    lootee.AddLoot(item);
                }
            }
            else
            {
                Logging.Log("Tried adding loot to non-existant Lootable");
            }
        }

        public void LootItem(Lootable lootee, Item item)
        {
            Program.ServerConnection.SendMessage(new AddItemsToInventoryMessage(item));

            this.Inventory.AddItem(item);
            lootee.RemoveItem(item);

            if (lootee.Items.Count() == 0)
            {
                this.StopLooting();
            }
        }

        public void HandleInput(InputUnion input)
        {
            if (GUI.HandleGUIInput(input))
            {
                return;
            }

            if (input.IsMouseInput)
            {
                var gameLocation = this.GameState.CurrentCamera.GLToGameCoordinate(input.Location);

                var entityIdentifier = GUI.EntityAt(gameLocation);

                if (entityIdentifier != null)
                {
                    var entity = this.GameState.GetEntityById(entityIdentifier);

                    if (entity != null)
                    {
                        if (input.MouseButton.Value == OpenTK.Input.MouseButton.Left && input.Direction == InputUnion.Directions.Up)
                        {
                            this.TryToTarget(entity);

                            entity.Interact();
                        }
                    }

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

        public void UseItem(Item item)
        {
            if (item.Equipable)
            {
                Program.ServerConnection.SendMessage(new GameEventsMessage(new EquipItemEvent(Hero.Info.Identifier, item)));
                return;
            }

            Program.ServerConnection.SendMessage(new GameEventsMessage(new UseItemEvent(Hero.Info.Identifier, item)));

            if (item.DepletesOnUse)
            {
                item.StackSize--;

                if (item.StackSize <= 0)
                {
                    Inventory.RemoveItem(item);
                }
            }
        }

        public void Loot(Lootable lootee)
        {
            if (this.Hero.DistanceTo(lootee) > LootingMaxDistance)
            {
                return;
            }

            if (this.Looting == lootee)
            {
                return;
            }

            if (this.Looting != null)
            {
                this.StopLooting();
            }

            this.Looting = lootee;
            GUI.ShowLoot(this.Looting);
        }

        public void RequestTransaction(Transaction transaction)
        {
            Logging.Log(transaction.TransactionIdentifier.ToString());
            Program.ServerConnection.SendMessage(new TransactionMessage(transaction));
        }

        public void HandleTransaction(Transaction transaction)
        {
            Transaction.GetTransaction(transaction.TransactionIdentifier).Execute(Inventory);
        }

        private void StopLooting()
        {
            GUI.CloseLootPanel();
            this.Looting = null;
        }

        private void TryToTarget(GameEntity target)
        {
            if (!target.Info.Targetable) { return; }

            this.Hero.Info.Target = target.Info.Identifier;
            GUI.SetTargetPanel(target);
        }

        private void Tick()
        {
            if (this.Hero != null)
            {
                this.HandleHeroMovement();

                if (this.Looting != null)
                {
                    if (this.Hero.DistanceTo(this.Looting) > LootingMaxDistance)
                    {
                        GUI.CloseLootPanel();
                        this.Looting = null;
                    }
                }
            }
        }

        private void HandleHeroMovement()
        {
            EntityMovement newMovements = new EntityMovement();

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
                newMovements.Direction = float.NaN;
            }
            else
            {
                newMovements.Direction = (float)direction;
            }

            if (!newMovements.Direction.Equals(this.PrevDirection))
            {
                var e = new UpdateMovementEvent(this.Hero.Info.Identifier, newMovements);

                Program.ServerConnection.SendMessage(new GameEventsMessage(e));
            }

            this.PrevDirection = newMovements.Direction;
        }

        private void AddLoot(Loot e, Item item)
        {
            e.Items.Add(item);
        }

        private void BeginCastAbility(Ability ability)
        {
            // if Hero has no target, try to cast spell on the Hero
            var targetIdentifier = this.Hero.Info.Target ?? this.Hero.Info.Identifier;

            var target = this.GameState.GetEntityById(targetIdentifier);

            if (target == null)
            {
                return;
            }

            if (!this.Hero.Abilities.CanCastAbility(ability, target, this.GameState))
            {
                return;
            }

            Program.ServerConnection.SendMessage(new GameEventsMessage(new BeginCastAbilityEvent(this.Hero, target, ability)));
        }

        private void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.CastAbility0:
                {
                    this.BeginCastAbility(this.Hero.Abilities.Abilities[0]);
                } break;

                case GameAction.CastAbility1:
                {
                    this.BeginCastAbility(this.Hero.Abilities.Abilities[1]);
                } break;

                case GameAction.CastAbility2:
                {
                    this.BeginCastAbility(this.Hero.Abilities.Abilities[2]);
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
                    throw new NotImplementedException();
#if false
                    GUI.Camera?.Zoom.ZoomIn();
#endif
                } break;

                case GameAction.CameraZoomOut:
                {
                        throw new NotImplementedException();
#if false
                        GUI.Camera?.Zoom.ZoomOut();
#endif
                } break;

                case GameAction.TogglePanel:
                {
                    foreach (var panel in GUI.GUIComponents.Where(p => p is Panel))
                    {
                        panel.Visible = !panel.Visible;
                    }
                } break;

                case GameAction.ToggleInventory:
                {
                    GUI.ToggleInventoryPanel();
                } break;

                case GameAction.ToggleEquipmentPanel:
                {
                    GUI.ToggleEquipmentPanel();
                } break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }

        private void UpdateLOS()
        {
#if true
            var inLOS = LineOfSight.CheckLOS(this.Hero, this.GameState.AllEntities);

            foreach (var e in inLOS)
            {
                e.LOSGraceTicks = 5;
            }

            foreach (var e in this.GameState.AllEntities)
            {
                e.LOSGraceTicks--;
                e.Info.Visible = e.LOSGraceTicks > 0;
            }
#endif
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
