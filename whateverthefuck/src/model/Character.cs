namespace whateverthefuck.src.model
{
    class MovementStruct
    {
        public bool Upwards { get; set; }
        public bool Downwards { get; set; }
        public bool Rightwards { get; set; }
        public bool Leftwards { get; set; }

    }

    class Character : GameEntity
    {
        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed = 0.01f;

        public void SetMovementUpwards(bool move)
        {
            Movements.Upwards = move;
        }

        public void SetMovementDownwards(bool move)
        {
            Movements.Downwards = move;
        }
        
        public void SetMovementLeftwards(bool move)
        {
            Movements.Leftwards = move;
        }

        public void SetMovementRightwards(bool move)
        {
            Movements.Rightwards = move;
        }

        public override void Step()
        {
            base.Step();

            if (Movements.Upwards)
            {
                Location.Y += MoveSpeed;
            }

            if (Movements.Downwards)
            {
                Location.Y -= MoveSpeed;
            }

            if (Movements.Leftwards)
            {
                Location.X -= MoveSpeed;
            }

            if (Movements.Rightwards)
            {
                Location.X += MoveSpeed;
            }
        }
    }
}
