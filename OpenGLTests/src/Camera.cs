using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

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

    public interface Camera
    {
        GameCoordinate Location { get; }
        Zoomer Zoom { get; }
    }

    public class FollowCamera : Camera
    {
        public Zoomer Zoom { get; } = new Zoomer();

        private Entity Following;

        public FollowCamera(Entity following)
        {
            Following = following;
        }

        public GameCoordinate Location
        {
            get
            {
                if (Following?.Location == null) return new GameCoordinate(0, 0);
                return Following.Location;
            }
        }
    }

    public class StaticCamera : Camera
    {
        public GameCoordinate Location { get; }
        public Zoomer Zoom { get; } = new Zoomer();

        public StaticCamera(GameCoordinate origin)
        {
            Location = origin;
        }
    }
}
