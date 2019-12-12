using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;


namespace whateverthefuck.src.view
{
    public class DrawAdapter
    {
        public DrawAdapter()
        {
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
    }
}
