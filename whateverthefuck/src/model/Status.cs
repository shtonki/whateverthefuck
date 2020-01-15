using System.Collections.Generic;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    public abstract class Status
    {
        public Status(EntityIdentifier applyor, int duration, int stacks, SpriteID spriteID)
        {
            this.Duration = duration;
            this.Stacks = stacks;
            this.Sprite = new Sprite(spriteID);
            this.Applyor = applyor;
        }

        public int Duration { get; set; }

        public int Stacks { get; set; }

        public EntityIdentifier Applyor { get; }

        public Sprite Sprite { get; }

        public abstract void ApplyTo(StatStruct status);

        public abstract IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state);
    }

    public class VulnerableStatus : Status
    {
        public VulnerableStatus(EntityIdentifier applyor, int duration, int stacks)
            : base(applyor, duration, stacks, SpriteID.status_Vulnerable)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
            status.DamageTaken *= 1.3f;
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            return null;
        }
    }

    public class SanicStatus : Status
    {
        public SanicStatus(EntityIdentifier applyor, int duration, int stacks)
            : base(applyor, duration, stacks, SpriteID.ability_Sanic)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
            status.MoveSpeed *= 2;
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            return null;
        }
    }

    public class SlowStatus : Status
    {
        public SlowStatus(EntityIdentifier applyor, int duration, int stacks)
            : base(applyor, duration, stacks, SpriteID.status_Slow)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
            status.MoveSpeed *= 0.2f;
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            return null;
        }
    }

    public class BurnStatus : Status
    {
        private const int BurnInterval = 100;

        public BurnStatus(EntityIdentifier applyor, int stacks)
            : base(applyor, BurnInterval, stacks, SpriteID.status_Burning)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            if (this.Duration % BurnInterval == 1)
            {
                var damage = this.Stacks;

                this.Stacks /= 2;

                if (this.Stacks > 0)
                {
                    this.Duration = BurnInterval;
                }

                var applyorEntity = state.GetEntityById(this.Applyor);
                return new GameEvent[] { new DealDamageEvent(applyorEntity, appliedTo, damage) };
            }
            else
            {
                return null;
            }
        }
    }
}
