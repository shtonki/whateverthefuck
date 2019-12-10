﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view
{
    public class Line : Drawable
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        public Line(GameEntity o, GameEntity e) : base(new GameCoordinate(0, 0))
        {
            X1 = o.Center.X;
            Y1 = o.Center.Y;
            X2 = e.Center.X;
            Y2 = e.Center.Y;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillLine(X1, Y1, X2, Y2, Color.White);
        }
    }


    public class Rectangle : Drawable
    {
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;

        public Color DrawColor;

        public Rectangle(GameEntity o, Color drawColor) : base(new GameCoordinate(0, 0))
        {
            X1 = o.Left;
            Y1 = o.Bottom;
            X2 = o.Right;
            Y2 = o.Top;
            DrawColor = drawColor;
        }

        public override void DrawMe(DrawAdapter drawAdapter)
        {
            drawAdapter.FillLine(X1, Y1, X2, Y1, DrawColor);
            drawAdapter.FillLine(X2, Y1, X2, Y2, DrawColor);
            drawAdapter.FillLine(X2, Y2, X1, Y2, DrawColor);
            drawAdapter.FillLine(X1, Y2, X1, Y1, DrawColor);
        }
    }

    public abstract class Drawable
    {
        protected float Rotation { get; set; } = 0;
        public Coordinate Location { get; set; }

        public bool Visible { get; set; } = true;

        public abstract void DrawMe(DrawAdapter drawAdapter);
        public void Draw(DrawAdapter drawAdapter)
        {
            if (Visible)
            {
                DrawMe(drawAdapter);
            }
        }


        protected Drawable(Coordinate location)
        {
            Location = location;
        }
    }

}
