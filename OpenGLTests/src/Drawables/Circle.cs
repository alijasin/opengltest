using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Entities;

namespace OpenGLTests.src.Drawables
{
    public interface IFollowable
    {
        GameCoordinate Location { get; set; }
        bool Visible { get; set; }
    }
    interface IClickable
    {
        Action<GameCoordinate> OnClick { get; set; }
        bool Contains(GameCoordinate point);
    }


    public abstract class RangeShape : Entity
    {
        protected IFollowable following;

        public abstract bool Contains(GameCoordinate point);
        public bool IsInfinite { get; set; } = false;

        public RangeShape(IFollowable following)
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

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public class Circle : Entity
    {
        public GLCoordinate Radius { get; set; }

        public Circle(GLCoordinate radius)
        {
            this.Radius = radius;
            Console.Write(radius);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);
            if (Visible && Radius != null) drawer.FillCircle(location.X, location.Y, Radius, Color);
        }

        public bool Contains(GameCoordinate point)
        {
            Console.WriteLine(Radius + " = " + "0");
            if (Radius == null) return false;

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
            if (circle == null) return;
            circle.Location = this.Location; //is this stupid? Currently we are drawing the circle's draw and not range circle. And the circle's draw uses its own location instaed of RangeShape's location(which is the following)
            if(Visible && IsInfinite == false) circle.DrawStep(drawer);
        }
    }
}