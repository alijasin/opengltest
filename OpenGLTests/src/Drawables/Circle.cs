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
    public interface IFollowable
    {
        GameCoordinate Location { get; set; }
    }
    interface IClickable
    {
        Action<GameCoordinate> OnClick { get; set; }
        bool Contains(GameCoordinate point);
    }

    public abstract class RangeShape : Entity
    {
        private IFollowable following;

        public abstract bool Contains(GameCoordinate point);
        public bool IsInfinite { get; set; } = false;

        protected RangeShape(IFollowable following)
        {
            this.following = following;
            this.Visible = false;
            GameState.Drawables.Add(this);
        }

        public override GameCoordinate Location
        {
            get
            {
                if (this.following?.Location == null) return new GameCoordinate(0, 0);
                return this.following.Location;
            }
        }

        public void SetFollowing(IFollowable follow)
        {
            this.following = follow;
        }
    }

    public class Circle : Entity
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

        public bool Contains(GameCoordinate point)
        {
            var x = Math.Abs(point.X - Location.X);
            var y = Math.Abs(point.Y - Location.Y);

            return x * x + y * y < Radius.X * Radius.X; //todo no ellipsis, this is circle
        }
    }

    public class RangeCircle : RangeShape
    {
        private Circle circle;

        public RangeCircle(GLCoordinate radius, IFollowable following) : base(following)
        {
            circle = new Circle(radius);
            //this.Visible = true;
            //circle.Visible = true;
        }

        public override bool Contains(GameCoordinate point)
        {
            return circle.Contains(point);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            circle.Location = this.Location; //is this stupid? Currently we are drawing the circle's draw and not range circle. And the circle's draw uses its own location instaed of RangeShape's location(which is the following unit)
            if(Visible && IsInfinite == false) circle.DrawStep(drawer);
        }
    }
}