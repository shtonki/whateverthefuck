namespace whateverthefuck.src.model
{
    public enum Statuses
    {
        Sanic,
    }

    public class Status
    {
        public Status(Statuses type, int duration, int stacks)
        {
            this.Type = type;
            this.Duration = duration;
            this.Stacks = stacks;
        }

        public Statuses Type { get; }

        public int Duration { get; set; }

        public int Stacks { get; set; }
    }
}
