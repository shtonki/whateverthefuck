using System.Drawing;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    public class PlayerCharacter : Character
    {
        public PlayerCharacter(EntityIdentifier identifier, CreationArgs args) : base(identifier, EntityType.PlayerCharacter, args)
        {
            DrawColor = Coloring.RandomColor();
        }
    }
}
