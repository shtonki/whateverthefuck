using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;

namespace whateverthefuck.src.view
{
    class Zoomer
    {
        public float CurrentZoom { get; set; } = 1.8f;

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

    abstract class Camera
    {
        public virtual Coordinate Location { get; }
        public virtual Zoomer Zoom { get; }

        public GLCoordinate GameToGLCoordinate(GameCoordinate gameCoordinate)
        {
            var x = gameCoordinate.X - GUI.Camera.Location.X;
            var y = -(gameCoordinate.Y - GUI.Camera.Location.Y);
            return new GLCoordinate(x, y);
        }

        public GameCoordinate GLToGameCoordinate(GLCoordinate glCoordinate)
        {
            throw new NotImplementedException();
        }
    }

    class FollowCamera : Camera
    {
        public override Zoomer Zoom { get; } = new Zoomer();

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
                return Following.Location;
            }
        }
    }

    class StaticCamera : Camera
    {
        public GameCoordinate Location { get; }
        public Zoomer Zoom { get; } = new Zoomer();

        public StaticCamera(GameCoordinate origin)
        {
            Location = origin;
        }
    }
}
