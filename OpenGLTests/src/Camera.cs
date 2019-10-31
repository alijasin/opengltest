using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public class Zoomer
    {
        public float CurrentZoom { get; set; } = 1;

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
        public virtual GameCoordinate Location { get; set; }
        Zoomer Zoom { get; }
        public GameCoordinate Speed { get; set; } = new GameCoordinate(0, 0);
        public GameCoordinate BaseSpeed { get; set; } = new GameCoordinate(0.01f, 0.01f);
        public virtual void Step()
        {

        }
    }

    public class HybridCamera : Camera
    {
        protected Entity following;
        private GameCoordinate movedDistance = new GameCoordinate(0,0);
        
        public HybridCamera(Entity following)
        {
            this.following = following;
        }

        public override GameCoordinate Location
        {
            get
            {
                if (this.following?.Location == null) return new GameCoordinate(0, 0);
                return this.following.Location + movedDistance;
            }
        }

        public override void Step()
        {
            if (Math.Abs(Speed.X) < BaseSpeed.X)
            {
                if (Math.Abs(movedDistance.X - BaseSpeed.X) < BaseSpeed.X) movedDistance.X = 0;
                else if (movedDistance.X - BaseSpeed.X > BaseSpeed.X)
                {
                    movedDistance.X -= BaseSpeed.X;
                }
                else if (movedDistance.X + BaseSpeed.X < -BaseSpeed.X)
                {
                    movedDistance.X += BaseSpeed.X;
                }
            }
            else
            {
                movedDistance.X += Speed.X;
            }


            if (Math.Abs(Speed.Y) < BaseSpeed.Y)
            {
                if (Math.Abs(movedDistance.Y - BaseSpeed.Y) < BaseSpeed.Y) movedDistance.Y = 0;
                else if (movedDistance.Y - BaseSpeed.Y > BaseSpeed.Y)
                {
                    movedDistance.Y -= BaseSpeed.Y;
                }
                else if (movedDistance.Y + BaseSpeed.Y < -BaseSpeed.Y)
                {
                    movedDistance.Y += BaseSpeed.Y;
                }
            }
            else
            {
                movedDistance.Y += Speed.Y;
            }
            
        }
    }

    public class FollowCamera : Camera
    {
        protected Entity following;

        public FollowCamera(Entity following)
        {
            this.following = following;
        }

        public override GameCoordinate Location
        {
            get
            {
                if (this.following?.Location == null) return new GameCoordinate(0, 0);
                return this.following.Location;
            }
        }
    }

    public class MovableCamera : Camera
    {
        public MovableCamera(GameCoordinate origin)
        {
            Location = origin;
        }

        public override void Step()
        {
            this.Location += Speed;
        }
    }

    /// <summary>
    /// Lmao dont use this
    /// </summary>
    public class ShakingCamera : FollowCamera
    {
        public ShakingCamera(Entity following) : base(following)
        {

        }

        public override GameCoordinate Location
        {
            get
            {
                if (this.following?.Location == null) return new GameCoordinate(0, 0);
                return new GameCoordinate(this.following.Location.X + RNG.BetweenZeroAndOne()/100 * RNG.NegativeOrPositiveOne(), this.following.Location.Y + RNG.BetweenZeroAndOne() / 100 * RNG.NegativeOrPositiveOne());
            }
        }
    }
}
