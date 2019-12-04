namespace whateverthefuck.src.model
{
    abstract class Character : GameEntity
    {
        public MovementStruct Movements { get; set; } = new MovementStruct();
        public float MoveSpeed = 0.01f;


        public Character(ControlInfo controlInfo, EntityIdentifier identifier) : base(controlInfo, identifier)
        {
            Movable = true;
        }

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

        public override GameCoordinate CalculateMovement()
        {
            var xMove = 0.0f;
            var yMove = 0.0f;

            if (Movements.Upwards) { yMove += MoveSpeed; }
            if (Movements.Downwards) { yMove -= MoveSpeed; }
            if (Movements.Leftwards) { xMove -= MoveSpeed; }
            if (Movements.Rightwards) { xMove += MoveSpeed; }

            return new GameCoordinate(xMove, yMove);
        }
    }

    class MovementStruct
    {
        public bool Upwards { get; set; }
        public bool Downwards { get; set; }
        public bool Rightwards { get; set; }
        public bool Leftwards { get; set; }

    }
}
