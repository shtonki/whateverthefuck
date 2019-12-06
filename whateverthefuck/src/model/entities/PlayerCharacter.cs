using System.Drawing;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model.entities
{
    public class PlayerCharacter : Character
    {
        public PlayerCharacter(EntityIdentifier identifier) : base(identifier)
        {
            DrawColor = Coloring.RandomColor();
        }
    }
}
