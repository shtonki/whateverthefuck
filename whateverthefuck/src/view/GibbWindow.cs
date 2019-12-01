﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace whateverthefuck.src.view
{
    class GibbWindow : GameWindow
    {

        private ManualResetEventSlim LoadResetEvent;

        public GibbWindow(ManualResetEventSlim loadResetEvent) : base(600, 600, new GraphicsMode(32, 24, 0, 32), "GibbWindow")
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            LoadResetEvent = loadResetEvent;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (LoadResetEvent != null) LoadResetEvent.Set();
        }

    }
}
