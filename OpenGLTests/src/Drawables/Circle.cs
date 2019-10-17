using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Entities;

namespace OpenGLTests.src.Drawables
{
    public class Circle : RangeShape
    {
        public GLCoordinate Radius { get; set; } = new GLCoordinate(0, 0);

        public Circle(GLCoordinate radius)
        {
            this.Radius = radius;
        }

        public override void DrawStep(DrawAdapter drawer)
        {

            GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);
            if (Visible) drawer.FillCircle(location.X, location.Y, Radius, Color.Crimson);
        }

        public override bool Contains(GameCoordinate point)
        {
            var x = Math.Abs(point.X - Location.X);
            var y = Math.Abs(point.Y - Location.Y);

            return x * x + y * y < Radius.X * Radius.X; //todo no ellipsis, this is circle
        }
    }

    public class FollowCircle : Circle
    {
        private Entity following;

        public FollowCircle(GLCoordinate radius, Entity following) : base(radius)
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

    public abstract class RangeShape : Entity
    {
        public abstract bool Contains(GameCoordinate point);
        public bool IsInfinite => Size == new GLCoordinate(0, 0);
        protected RangeShape()
        {
        }
        //public ActionMarker Marker { get; set; }
    }

    interface IClickable
    {
        Action<GameCoordinate> OnClick { get; set; }
        bool Contains(GameCoordinate point);
    }

    public class RangeCircle : Circle, IClickable
    {
        public Action<GameCoordinate> OnClick { get; set; }

        public RangeCircle(GLCoordinate radius) : base(radius)
        {
            this.Visible = false;
            OnClick = (GameCoordinate gc) =>
            {
                //this.Marker.Location = CoordinateFuckery.ClickToGLRelativeToCamera(gc, this.Location);

            };
        }
    }

}