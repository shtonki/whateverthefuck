namespace whateverthefuck.src.model.entities
{
    using System.Drawing;

    public class NPC : Character
    {
        public NPC(EntityIdentifier identifier, CreationArgs args)
            : base(identifier, EntityType.NPC, args)
        {
            this.DrawColor = Color.Red;
            this.Movable = true;
            this.MoveSpeed = 0.001f;
        }
    }
}
