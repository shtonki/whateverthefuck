using System.Collections.Generic;
using System.Drawing;
using whateverthefuck.src.view;

namespace whateverthefuck.src.util
{
    public enum SpriteID
    {
        testSprite1,


    }

    public static class ImageLoader
    {
        private static Dictionary<SpriteID, int> GLIDs = new Dictionary<SpriteID, int>();
        private static Dictionary<SpriteID, Image> Images = new Dictionary<SpriteID, Image>();

        public static void Init()
        {
            Images[SpriteID.testSprite1] = new Bitmap(Properties.Resources.kappa);
            
            //foreach (var image in Enum.GetValues(typeof(SpriteID)).Cast<SpriteID>())
            foreach (var img in Images)
            {
                GetBinding(img.Key);
            }
        }

        public static Image GetImage(SpriteID id)
        {
            if (Images.ContainsKey(id)) return Images[id];
            else return null;
        }

        /// <summary>
        /// Gets the int Used by OpenGL to identify the texture bound from the given Images
        /// </summary>
        /// <param name="spriteId"></param>
        /// <returns></returns>
        public static int GetBinding(SpriteID spriteId)
        {
            if (GLIDs.ContainsKey(spriteId))
            {
                return GLIDs[spriteId];
            }

            var loaded = DrawAdapter.CreateTexture(Images[spriteId]);
            GLIDs[spriteId] = loaded;
            return loaded;
        }
    }
}