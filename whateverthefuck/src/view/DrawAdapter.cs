using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;


namespace whateverthefuck.src.view
{
    class DrawAdapter
    {
        public DrawAdapter()
        {
        }

        public void blowme()
        {
            GL.Color4(Color.Black);

            GL.Begin(PrimitiveType.Quads);


            GL.Vertex2(0f, 0f);
            GL.Vertex2(0f, 1f);
            GL.Vertex2(1f, 1f);
            GL.Vertex2(1f, 0f);

            GL.End();
        }

        public void fillRectangle(float x1, float y1, float x2, float y2, Color c)
        {

            GL.Color4(c);

            GL.Begin(PrimitiveType.Quads);


            GL.Vertex2(x1, y1);
            GL.Vertex2(x1, y2);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x2, y1);

            GL.End();
        }
    }
}
