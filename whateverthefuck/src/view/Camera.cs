using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view
{
    public class Zoomer
    {
        public float CurrentZoom { get; set; } = 1.0f;

        private float MinZoom { get; } = 0.4f;
        private float MaxZoom { get; } = 1.8f;
        private float ZoomStepSize { get; } = 0.2f;

        public void ZoomIn()
        {
            if (CurrentZoom - ZoomStepSize < MinZoom) CurrentZoom = MinZoom;
            else CurrentZoom -= ZoomStepSize;
        }

        public void ZoomOut()
        {
            if (CurrentZoom + ZoomStepSize > MaxZoom) CurrentZoom = MaxZoom;
            else CurrentZoom += ZoomStepSize;
        }
    }

    public abstract class Camera
    {
        public virtual Coordinate Location { get; protected set; }
        public Zoomer Zoom { get; } = new Zoomer();

        public GLCoordinate GameToGLCoordinate(GameCoordinate gameCoordinate)
        {
            var x = gameCoordinate.X - GUI.Camera.Location.X;
            var y = -(gameCoordinate.Y - GUI.Camera.Location.Y);
            return new GLCoordinate(x, y);
        }

        public GameCoordinate GLToGameCoordinate(GLCoordinate glCoordinate)
        {
            return new GameCoordinate((glCoordinate.X / Zoom.CurrentZoom + Location.X), (glCoordinate.Y / Zoom.CurrentZoom - Location.Y));
        }
    }

    class FollowCamera : Camera
    {
        private GameEntity Following;

        public FollowCamera(GameEntity following)
        {
            Following = following;
        }

        public override Coordinate Location
        {
            get
            {
                if (Following?.Location == null) return new GameCoordinate(0, 0);
                return Following.Center;
            }
        }
    }

    class StaticCamera : Camera
    {
        public StaticCamera(GameCoordinate origin)
        {
            Location = origin;
        }
    }
}
