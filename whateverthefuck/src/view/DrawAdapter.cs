using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using whateverthefuck.src.util;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;


namespace whateverthefuck.src.view
{
    public class DrawAdapter
    {
        public DrawAdapter()
        {
        }


        public void PushMatrix()
        {
            GL.PushMatrix();
        }

        public void PopMatrix()
        {
            GL.PopMatrix();
        }

        public void Translate(float x, float y)
        {
            GL.Translate(x, y, 0);
        }

        public void Rotate(float angle)
        {
            GL.Rotate(angle, OpenTK.Vector3d.UnitZ);
        }

        public void Scale(float x, float y)
        {
            GL.Scale(x, y, 0);
        }

        public void ActivateScissor(int x, int y, int width, int height)
        {
            GL.Scissor(x, y, width, height);
            GL.Enable(EnableCap.ScissorTest);
        }

        public void DeactivateScissor()
        {
            GL.Disable(EnableCap.ScissorTest);
        }

        public void FillRectangle(float x1, float y1, float x2, float y2, Color c)
        { 
            GL.Color4(c);

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x2, y1);

            GL.End();
        }

        public void TraceRectangle(float x1, float y1, float x2, float y2, Color c, float width = 1)
        {
            GL.Color4(c);

            GL.LineWidth(width);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x2, y1);

            GL.End();
        }

        public void FillLine(float xorg, float yorg, float xend, float yend, Color c)
        {
            GL.PushAttrib(AttribMask.CurrentBit);
            GL.LineWidth(4);
            GL.Color4(c);
            GL.Begin(PrimitiveType.Lines);

            GL.Vertex2(xorg, yorg);
            GL.Vertex2(xend, yend);

            GL.End();
            GL.LineWidth(1);
            GL.PopAttrib();
        }

        public void DrawSprite(SpriteID sid, float x, float y, float w, float h)
        {
            GL.PushMatrix();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.Color4(Color.White);

            GL.BindTexture(TextureTarget.Texture2D, ImageLoader.GetBinding(sid));
            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(x, y);

            GL.TexCoord2(0, 1);
            GL.Vertex2(x, y+h);

            GL.TexCoord2(1, 1);
            GL.Vertex2(x+w, y+h);

            GL.TexCoord2(1, 0);
            GL.Vertex2(x+w, y);

            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);

            GL.PopMatrix();
        }

        /// <summary>
        /// Binds an Image in OpenGL 
        /// </summary>
        /// <param name="image">The image to be bound to a texture</param>
        /// <returns>The integer Used by OpenGL to identify the created texture</returns>
        public static int CreateTexture(Image image)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(image);

            BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            return id;
        }
    }
}
