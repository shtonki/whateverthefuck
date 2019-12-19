namespace whateverthefuck.src.view
{
    using System.Drawing;
    using whateverthefuck.src.model;
    using whateverthefuck.src.util;

    public class Sprite
    {
        public Sprite(SpriteID sid)
        {
            this.ID = sid;
        }

        public SpriteID ID { get; protected set; }

        public static Sprite GetSprite(AbilityType abilityType)
        {
            SpriteID id;
            switch (abilityType)
            {
                case AbilityType.Fireball:
                    {
                        id = SpriteID.ability_Fireball;
                    }
                    break;

                default:
                    {
                        id = SpriteID.testSprite1;
                    }
                    break;
            }

            return new Sprite(id);
        }

        public Image Image() => ImageLoader.GetImage(this.ID);

    }
}
