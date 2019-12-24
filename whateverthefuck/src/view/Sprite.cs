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

        public static Sprite GetAbilitySprite(AbilityType abilityType)
        {
            SpriteID id;
            switch (abilityType)
            {
                case AbilityType.Fireball:
                {
                    id = SpriteID.ability_Fireball;
                } break;

                default:
                {
                    id = SpriteID.testSprite1;
                } break;
            }

            return new Sprite(id);
        }

        public static Sprite GetItemSprite(Item item)
        {
            SpriteID id;
            switch (item.Type)
            {
                case ItemType.BronzeDagger:
                {
                    id = SpriteID.item_Bronze_Dagger;
                } break;

                case ItemType.Banana:
                {
                    id = SpriteID.item_Banana;
                } break;

                default:
                {
                    id = SpriteID.testSprite1;
                } break;
            }

            return new Sprite(id);
        }

        public Image Image() => ImageLoader.GetImage(this.ID);

    }
}
