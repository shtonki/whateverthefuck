using System.Collections.Generic;
using System.Linq;
using whateverthefuck.src.util;

namespace whateverthefuck.src.model.entities
{
    public class Brain
    {
        public GameEvent Use(NPC npc, GameState gameState)
        {
            if (npc.LastDamageTaken != null)
            {
                var target = gameState.GetEntityById(npc.LastDamageTaken.AttackerId);
                var castableAbilities = npc.CastableAbilities(target, gameState);

                if (castableAbilities.Count() > 0)
                {
                    return new BeginCastAbilityEvent(npc, target, castableAbilities.ElementAt(RNG.IntegerBetween(0, castableAbilities.Count())));
                }

                MovementStruct ms = new MovementStruct();
                ms.FollowId = npc.LastDamageTaken.AttackerId;
                return new UpdateMovementEvent(npc.Identifier.Id, ms);
            }

            return null;
        }
    }
}
