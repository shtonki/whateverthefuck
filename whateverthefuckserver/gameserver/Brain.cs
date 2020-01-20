using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public class Brain
    {
        private const bool EnableLogging = true;

        public Brain(GameEntity head)
        {
            head.OnDamaged += (dde, gs) =>
            {
                if (ThreatTable.Count == 0)
                {
                    Tagger = dde.AttackerIdentifier;
                }


                if (!ThreatTable.ContainsKey(dde.AttackerIdentifier))
                {
                    ThreatTable[dde.AttackerIdentifier] = 0;
                }

                ThreatTable[dde.AttackerIdentifier] += dde.Damage;

                int maxThreat = 0;

                if (TopThreat != null)
                {
                    maxThreat = ThreatTable[TopThreat];
                }

                foreach (var kpv in ThreatTable)
                {
                    if (kpv.Value > maxThreat)
                    {
                        TopThreat = kpv.Key;
                        break;
                    }
                }
            };
        }

        public EntityIdentifier TopThreat { get; private set; }

        public EntityIdentifier Tagger { get; private set; }

        private BrainState State { get; set; } = BrainState.Idle;
        
        private Dictionary<EntityIdentifier, int> ThreatTable { get; } = new Dictionary<EntityIdentifier, int>();

        private float HealthPercentageIntoDefensiveCutoff { get; set; } = 0.35f;

        public GameEvent[] Use(GameEntity entity, GameState gameState)
        {
            this.UpdateState(entity, gameState);

            switch (State)
            {
                case BrainState.Offensive:
                {
                    if (TopThreat != null)
                    {
                        var target = gameState.GetEntityById(TopThreat);

                        if (target == null)
                        {
                            TopThreat = null;
                            return null;
                        }

                        var ability = GetRandomStateAppropriateAbility(entity, target, gameState);

                        if (ability != null)
                        {
                            EntityMovement movementContainer = new EntityMovement();
                            return new GameEvent[]
                            {
                                new UpdateMovementEvent(entity.Info.Identifier, movementContainer),
                                new BeginCastAbilityEvent(entity, target, ability),
                            };
                        }

                        if (entity.Movements.FollowId?.Id != TopThreat.Id)
                        {
                            EntityMovement movementContainer = new EntityMovement();
                            movementContainer.FollowId = TopThreat;
                            return new GameEvent[] 
                            {
                                new UpdateMovementEvent(entity.Info.Identifier, movementContainer)
                            };
                        }
                    }
                } break;

                case BrainState.Defensive:
                {
                    var ability = GetRandomStateAppropriateAbility(entity, entity, gameState);

                    if (ability != null)
                    {
                        EntityMovement movementContainer = new EntityMovement();
                        return new GameEvent[]
                        {
                        new UpdateMovementEvent(entity.Info.Identifier, movementContainer),
                        new BeginCastAbilityEvent(entity, entity, ability),
                        };
                    }
                } break;

                case BrainState.Wander:
                {
                    if (gameState.StepCounter % 300 == 0)
                    {
                        EntityMovement movements = new EntityMovement();
                        movements.Direction = RNG.BetweenZeroAndOne() * 6.28f;
                        return new GameEvent[] { new UpdateMovementEvent(entity.Info.Identifier, movements) };
                    }

                    if (gameState.StepCounter % 300 == 50)
                    {
                        EntityMovement movements = new EntityMovement();
                        return new GameEvent[] { new UpdateMovementEvent(entity.Info.Identifier, movements) };
                    }
                } break;
            }

            return null;
        }

        private Ability GetRandomStateAppropriateAbility(GameEntity entity, GameEntity target, GameState gameState)
        {
            var castableAbilities = GetStateAppropriateSpells(entity, target, gameState);

            if (castableAbilities != null && castableAbilities.Count() > 0)
            {
                return castableAbilities.ElementAt(RNG.IntegerBetween(0, castableAbilities.Count()));
            }

            return null;
        }

        private IEnumerable<Ability> GetStateAppropriateSpells(GameEntity entity, GameEntity target, GameState gameState)
        {
            var castableAbilities = entity.Abilities.CastableAbilities(target, gameState);

            switch (State)
            {
                case BrainState.Offensive:
                {
                    return castableAbilities.Where(a => a.HasTag(NPCBrainAbilityTags.Offensive));
                }

                case BrainState.Defensive:
                {
                    return castableAbilities.Where(a => a.HasTag(NPCBrainAbilityTags.Defensive));
                }
            }

            return null;
        }

        private void UpdateState(GameEntity entity, GameState gameState)
        {
            switch (State)
            {
                case BrainState.Idle:
                {
                    if (EnableLogging) { Logging.Log(entity.Info.Identifier.Id + ": Idle -> Wander"); }
                    State = BrainState.Wander;
                } break;

                case BrainState.Wander:
                {
                    if (TopThreat != null)
                    {
                        if (EnableLogging) { Logging.Log(entity.Info.Identifier.Id + ": Wander -> Offensive because TopThread is something."); }
                        State = BrainState.Offensive;
                        }
                } break;

                case BrainState.Offensive:
                {
                    var healthPercentage = (float)entity.Status.ReadCurrentStats.Health / entity.Status.ReadCurrentStats.MaxHealth;
                    if (healthPercentage < HealthPercentageIntoDefensiveCutoff)
                    {
                        if (EnableLogging) { Logging.Log(entity.Info.Identifier.Id + ": Offensive -> Defensive because health < 50%."); }
                        State = BrainState.Defensive;
                        }
                } break;

                case BrainState.Defensive:
                {
                    var healthPercentage = (float)entity.Status.ReadCurrentStats.Health / entity.Status.ReadCurrentStats.MaxHealth;
                    if (healthPercentage > HealthPercentageIntoDefensiveCutoff)
                    {
                        if (EnableLogging) { Logging.Log(entity.Info.Identifier.Id + ": Defensive -> Offensive because health > 50%."); }
                        State = BrainState.Offensive;
                    }
                } break;
            }
        }

        enum BrainState
        {
            Idle,

            Wander,

            Offensive,
            Defensive,
        }
    }
}
