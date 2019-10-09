using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Entities;

namespace OpenGLTests.src.Drawables
{
    public class Circle : RangeShape
    {
        public GLCoordinate Radius { get; set; } = new GLCoordinate(0.3f, 0.3f);

        public Circle(GLCoordinate radius)
        {
            this.Radius = radius;
        }

        public override void Draw(DrawAdapter drawer)
        {

            GLCoordinate location = Location.ToGLCoordinate(GameState.Cam.Location);
            if (Visible) drawer.FillCircle(location.X, location.Y, Radius, Color.Crimson);
        }

        public override bool Contains(GameCoordinate point)
        {
            var x = Math.Abs(point.X - Location.X);
            var y = Math.Abs(point.Y - Location.Y);

            return x * x + y * y < Radius.X * Radius.X; //todo no ellipsis, this is circle
        }
//        public override bool Contains(GameCoordinate point)
//        {
//            var x = Math.Abs(point.X - Location.X);
//            var y = Math.Abs(point.Y - Location.Y);
//            //todo move this to somewhere else and fuck yourself.
//            GLCoordinate clicked = new GLCoordinate(x * 2 / GibbWindow.WIDTH - 1, y * 2 / GibbWindow.HEIGHT - 1);
//            return clicked.X * clicked.X + clicked.Y * clicked.Y < Radius.X * Radius.X; //todo no ellipsis, this is circle
//            //return (clicked.X * clicked.X)*(Radius.Y * Radius.Y) + (clicked.Y * clicked.Y)*(Radius.X * Radius.X) <= (Radius.X*Radius.X*Radius.Y*Radius.Y);
//        }
    }

    public abstract class RangeShape : Entity
    {
        public abstract bool Contains(GameCoordinate point);
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
