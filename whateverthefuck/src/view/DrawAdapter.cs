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
        
        public void FillRectangle(float x1, float y1, float x2, float y2, Color c, float rotation = 0)
        { 
            GL.PushMatrix();

            GL.Color4(c);

            GL.Translate(x1 +(x2-x1)/2, y1 +(y2-y1)/2, 0);
            GL.Rotate(rotation, 0, 0, 1);
            GL.Translate(-(x1 + (x2 - x1) / 2), -(y1+(y2 - y1) / 2), 0);

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x2, y1);

            GL.End();
            GL.PopMatrix();
        }
    }
}
