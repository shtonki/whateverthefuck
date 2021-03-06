﻿namespace whateverthefuck.src.view
{
    using whateverthefuck.src.model;

    public abstract class Camera
    {
        private Coordinate lockedLocation;

        public Coordinate Location => this.lockedLocation ?? this.CurrentLocation;

        public Zoomer Zoom { get; } = new Zoomer();

        protected virtual Coordinate CurrentLocation { get; set; }

        public GLCoordinate GameToGLCoordinate(GameCoordinate gameCoordinate)
        {
            var x = gameCoordinate.X - this.Location.X;
            var y = gameCoordinate.Y - this.Location.Y;
            return new GLCoordinate(x, y);
        }

        public GameCoordinate GLToGameCoordinate(GLCoordinate glCoordinate)
        {
            return new GameCoordinate(
                (glCoordinate.X / this.Zoom.CurrentZoom) + this.Location.X,
                (glCoordinate.Y / this.Zoom.CurrentZoom) + this.Location.Y);
        }

        public void Lock()
        {
            this.lockedLocation = this.CurrentLocation;
        }

        public void Unlock()
        {
            this.lockedLocation = null;
        }
    }

    public class Zoomer
    {
        public float CurrentZoom { get; set; } = 1.0f;

        private float MinZoom { get; } = 0.4f;

        private float MaxZoom { get; } = 1.8f;

        private float ZoomStepSize { get; } = 0.2f;

        public void ZoomIn()
        {
            if (this.CurrentZoom - this.ZoomStepSize < this.MinZoom)
            {
                this.CurrentZoom = this.MinZoom;
            }
            else
            {
                this.CurrentZoom -= this.ZoomStepSize;
            }
        }

        public void ZoomOut()
        {
            if (this.CurrentZoom + this.ZoomStepSize > this.MaxZoom)
            {
                this.CurrentZoom = this.MaxZoom;
            }
            else
            {
                this.CurrentZoom += this.ZoomStepSize;
            }
        }
    }

    internal class FollowCamera : Camera
    {
        private GameEntity following;

        public FollowCamera(GameEntity following)
        {
            this.following = following;
        }

        protected override Coordinate CurrentLocation
        {
            get
            {
                if (this.following?.Location == null)
                {
                    return new GameCoordinate(0, 0);
                }

                return this.following.Center;
            }
        }
    }

    internal class StaticCamera : Camera
    {
        public StaticCamera(GameCoordinate origin)
        {
            this.CurrentLocation = origin;
        }
    }
}
