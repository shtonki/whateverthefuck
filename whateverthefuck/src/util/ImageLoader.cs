﻿namespace whateverthefuck.src.util
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using OpenTK.Graphics.OpenGL;
    using whateverthefuck.src.view;
    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public enum SpriteID
    {
        testSprite1,

        ability_Bite,
        ability_Fireball,
        ability_Sanic,
        ability_Fireburst,
        ability_Mend,

        item_Bronze_Dagger,
        item_Bronze_Helmet,
        item_Banana,

        floor_Wood0,

        door_Stone0,

        wall_Stone0,

        player_Player0,

        npc_Dog,

        projectile_Fireball,

        status_Slow,
        status_Vulnerable,
        status_Burning,
    }

    public static class ImageLoader
    {
        private static Dictionary<SpriteID, int> glIDs = new Dictionary<SpriteID, int>();
        private static Dictionary<SpriteID, Image> images = new Dictionary<SpriteID, Image>();

        public static void Init()
        {
            images[SpriteID.testSprite1] = new Bitmap(Properties.Resources.kappa);

            images[SpriteID.ability_Bite] = new Bitmap(Properties.Resources.ability_fireball);
            images[SpriteID.ability_Fireball] = new Bitmap(Properties.Resources.ability_fireball);
            images[SpriteID.ability_Sanic] = new Bitmap(Properties.Resources.ability_sanic);
            images[SpriteID.ability_Fireburst] = new Bitmap(Properties.Resources.ability_fireburst);
            images[SpriteID.ability_Mend] = new Bitmap(Properties.Resources.ability_mend);

            images[SpriteID.item_Banana] = new Bitmap(Properties.Resources.item_banana);
            images[SpriteID.item_Bronze_Dagger] = new Bitmap(Properties.Resources.item_bronze_dagger);
            images[SpriteID.item_Bronze_Helmet] = new Bitmap(Properties.Resources.item_bronze_helmet);

            images[SpriteID.floor_Wood0] = new Bitmap(Properties.Resources.floor_wood0);

            images[SpriteID.door_Stone0] = new Bitmap(Properties.Resources.door_stone0);

            images[SpriteID.wall_Stone0] = new Bitmap(Properties.Resources.wall_stone0);

            images[SpriteID.player_Player0] = new Bitmap(Properties.Resources.player_player0);

            images[SpriteID.npc_Dog] = new Bitmap(Properties.Resources.npc_dog);

            images[SpriteID.projectile_Fireball] = new Bitmap(Properties.Resources.projectile_fireball);

            images[SpriteID.status_Slow] = new Bitmap(Properties.Resources.status_slow);
            images[SpriteID.status_Vulnerable] = new Bitmap(Properties.Resources.status_vulnerable);
            images[SpriteID.status_Burning] = new Bitmap(Properties.Resources.status_burning);

            foreach (var img in images)
            {
                GetBinding(img.Key);
            }
        }

        public static Image GetImage(SpriteID id)
        {
            if (images.ContainsKey(id))
            {
                return images[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the int Used by OpenGL to identify the texture bound from the given Images.
        /// </summary>
        /// <param name="spriteId">The SpriteID for which we are to look up the texture identifier.</param>
        /// <returns>The texture identifier generated by the GL context.</returns>
        public static int GetBinding(SpriteID spriteId)
        {
            if (glIDs.ContainsKey(spriteId))
            {
                return glIDs[spriteId];
            }

            var loaded = CreateTexture(images[spriteId]);
            glIDs[spriteId] = loaded;
            return loaded;
        }

        /// <summary>
        /// Binds an Image in OpenGL.
        /// </summary>
        /// <param name="image">The image to be bound to a texture.</param>
        /// <returns>The integer Used by OpenGL to identify the created texture.</returns>
        private static int CreateTexture(Image image)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(image);

            BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                data.Width,
                data.Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            return id;
        }
    }
}