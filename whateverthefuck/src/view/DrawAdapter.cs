namespace whateverthefuck.src.view
{
    using System.Drawing;
    using OpenTK.Graphics.OpenGL;
    using whateverthefuck.src.util;

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

        public void ActivateScissor(GLCoordinate location, GLCoordinate size)
        {
            var locScreenCoords = GUI.TranslateGLToScreenCoordinates(location);
            var sizeScreenCoords = GUI.TranslateGLToScreenCoordinates(new GLCoordinate(size.X - 1, size.Y - 1));

            GL.Scissor(
                locScreenCoords.X,
                locScreenCoords.Y,
                sizeScreenCoords.X,
                sizeScreenCoords.Y);
            GL.Enable(EnableCap.ScissorTest);
        }

        public void ActivateScissor(int x, int y, int width, int height)
        {
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

        public void DrawSprite(float x, float y, float w, float h, Sprite sprite)
        {
            GL.PushMatrix();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.Color4(Color.White);

            GL.BindTexture(TextureTarget.Texture2D, ImageLoader.GetBinding(sprite.ID));
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(1, 1);
            GL.Vertex2(x, y);

            GL.TexCoord2(1, 0);
            GL.Vertex2(x, y + h);

            GL.TexCoord2(0, 0);
            GL.Vertex2(x + w, y + h);

            GL.TexCoord2(0, 1);
            GL.Vertex2(x + w, y);

            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);

            GL.PopMatrix();
        }
    }
}
