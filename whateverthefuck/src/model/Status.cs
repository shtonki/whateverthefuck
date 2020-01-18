namespace whateverthefuck.src.model
{
    using System;
    using System.Collections.Generic;
    using whateverthefuck.src.util;
    using whateverthefuck.src.view;

    public abstract class Status
    {
        public Status(StatusType type, EntityIdentifier applyor, int duration, int stacks)
        {
            this.Type = type;
            this.Duration = duration;
            this.Stacks = stacks;
            this.Applyor = applyor;
            this.Sprite = new Sprite(SpriteID.testSprite1);
        }

        public StatusType Type { get; }

        public int Duration { get; set; }

        public int Stacks { get; set; }

        public EntityIdentifier Applyor { get; }

        public Sprite Sprite { get; protected set; }

        protected StackingModes DurationStacking { get; set; } = StackingModes.Max;

        protected StackingModes StackCountStacking { get; set; } = StackingModes.Max;

        public void Stack(Status otherStatus)
        {
            this.Duration = this.ApplyStackingMode(this.Duration, otherStatus.Duration, this.DurationStacking);
            this.Stacks = this.ApplyStackingMode(this.Stacks, otherStatus.Stacks, this.StackCountStacking);
        }

        public abstract void ApplyTo(StatStruct status);

        public abstract IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state);

        private int ApplyStackingMode(int i1, int i2, StackingModes mode)
        {
            switch (mode)
            {
                case StackingModes.Max:
                {
                    return Math.Max(i1, i2);
                }

                case StackingModes.Additive:
                {
                    return i1 + i2;
                }

                case StackingModes.None:
                {
                    return i1;
                }

                default:
                {
                    Logging.Log("Applying unknown StackingMode", Logging.LoggingLevel.Error);
                    return i1;
                }
            }
        }

        protected enum StackingModes
        {
            Max,
            Additive,
            None,
        }
    }

    public enum StatusType
    {
        Vulnerable,
        Sanic,
        Slowed,
        Burning,
    }

    public class VulnerableStatus : Status
    {
        public VulnerableStatus(EntityIdentifier applyor, int duration, int stacks)
            : base(StatusType.Vulnerable, applyor, duration, stacks)
        {
            this.DurationStacking = StackingModes.None;
            this.StackCountStacking = StackingModes.Additive;
            this.Sprite = new Sprite(SpriteID.status_Vulnerable);
        }

        public override void ApplyTo(StatStruct status)
        {
            status.DamageTakenMultiplier *= 1 + (this.Stacks * 0.01f);
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            return null;
        }
    }

    public class SanicStatus : Status
    {
        public SanicStatus(EntityIdentifier applyor, int duration, int stacks)
            : base(StatusType.Sanic, applyor, duration, stacks)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
            status.MoveSpeed *= 1 + (this.Stacks * 0.01f);
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            return null;
        }
    }

    public class SlowedStatus : Status
    {
        public SlowedStatus(EntityIdentifier applyor, int duration, int stacks)
            : base(StatusType.Slowed, applyor, duration, stacks)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
            status.MoveSpeed *= 1 - (this.Stacks * 0.01f);
        }

        public override IEnumerable<GameEvent> Tick(GameEntity appliedTo, GameState state)
        {
            return null;
        }
    }

    public class BurningStatus : Status
    {
        private const int BurnInterval = 100;

        public BurningStatus(EntityIdentifier applyor, int stacks)
            : base(StatusType.Burning, applyor, BurnInterval, stacks)
        {
            this.DurationStacking = StackingModes.None;
            this.StackCountStacking = StackingModes.Additive;
            this.Sprite = new Sprite(SpriteID.status_Burning);
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
                
                // if applying entity has disappeared, it does self damage.
                return new GameEvent[] { new DealDamageEvent(applyorEntity ?? appliedTo, appliedTo, damage) };
            }
            else
            {
                return null;
            }
        }
    }
}
