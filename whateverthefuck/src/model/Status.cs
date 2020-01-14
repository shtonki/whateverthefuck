namespace whateverthefuck.src.model
{
    public abstract class Status
    {
        public Status(int duration, int stacks)
        {
            this.Duration = duration;
            this.Stacks = stacks;
        }

        public int Duration { get; set; }

        public int Stacks { get; set; }

        public abstract void ApplyTo(StatStruct status);
    }

    public class SanicStatus : Status
    {
        public SanicStatus(int duration, int stacks)
            : base(duration, stacks)
        {
        }

        public override void ApplyTo(StatStruct status)
        {
            status.MoveSpeed *= 2;
        }
    }

}
