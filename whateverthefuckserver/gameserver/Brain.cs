using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public class Brain
    {
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

        private Dictionary<EntityIdentifier, int> ThreatTable = new Dictionary<EntityIdentifier, int>();

        public GameEvent Use(GameEntity entity, GameState gameState)
        {
            if (TopThreat != null)
            {
                var target = gameState.GetEntityById(TopThreat);
                var castableAbilities = entity.Abilities.CastableAbilities(target, gameState);

                if (castableAbilities.Count() > 0)
                {
                    return new BeginCastAbilityEvent(entity, target, castableAbilities.ElementAt(RNG.IntegerBetween(0, castableAbilities.Count())));
                }

                if (entity.Movements.FollowId?.Id != TopThreat.Id)
                {
                    EntityMovement movementContainer = new EntityMovement();
                    movementContainer.FollowId = TopThreat;
                    return new UpdateMovementEvent(entity.Info.Identifier, movementContainer);
                }
            }

            return null;
        }
    }
}
