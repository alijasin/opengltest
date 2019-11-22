using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;
using Console = System.Console;

namespace OpenGLTests.src.Drawables
{
    interface ILeftClickable
    {
        [JsonIgnore]
        Action<Hero, GameCoordinate> OnLeftClick { get; set; }
        bool LeftClickFilter(Hero hero, GameCoordinate point);
    }

    interface IRightClickable
    {
        [JsonIgnore]
        Action<Hero, GameCoordinate> OnRightClick { get; set; }
        bool RightClickFilter(Hero hero, GameCoordinate point);
    }

    public interface IShape
    {
        bool Contains(GameCoordinate point, GameCoordinate location, int angle);
        void DrawStep(DrawAdapter drawer, GameCoordinate location, int angle);
    }

    public class RangeShape : Indicator
    {
        public IShape Shape { get; set; }
        public virtual bool IsInfinite { get; set; } = false;
        public Entity Following { get; set; }

        public RangeShape(IShape shape, Entity following)
        {
            this.Visible = false;
            this.Shape = shape;
            this.Following = following;

            GameState.Drawables.Add(this);
        }

        public override GameCoordinate Location
        {
            get
            {
                if (this.Following?.Location == null) return new GameCoordinate(0, 0);
                return this.Following.Location;
            }
        }

        public bool Contains(GameCoordinate point)
        {
            if (IsInfinite) return true;
            return Shape.Contains(point, Location, Following.FacingAngle);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (IsInfinite) return;
            if (Visible)
            {
                Shape.DrawStep(drawer, Location, Following.FacingAngle);
            }
        }

        //todo: these all assume rectangles, which might not be acceptable later on.
        public float Left => this.Location.X - (this.Shape as Rectangle).Dimensions.X/2;
        public float Right => this.Location.X + (this.Shape as Rectangle).Dimensions.X/2;
        public float Bottom => this.Location.Y - (this.Shape as Rectangle).Dimensions.Y/2;
        public float Top => this.Location.Y + (this.Shape as Rectangle).Dimensions.Y/2;

        public float XCenter => Left + (Right - Left) / 2;
        public float YCenter => Bottom + (Top - Bottom) / 2;
    }
    
    public class Circle : IShape
    {
        public GLCoordinate Radius { get; set; }

        //should probably be float and turned into GLCoordinate,
        //or rename to elipsis
        public Circle(GLCoordinate radius)
        {
            this.Radius = radius;
        }

        public bool Contains(GameCoordinate point, GameCoordinate location, int angle = 0)
        {
            var x = Math.Abs(point.X - location.X);
            var y = Math.Abs(point.Y - location.Y);

            return x * x + y * y < Radius.X * Radius.X; //todo no ellipsis, this is circle
        }

        public void DrawStep(DrawAdapter drawer, GameCoordinate location, int angle)
        {
            GLCoordinate locationx = location.ToGLCoordinate();
            drawer.DrawCircle(locationx.X, locationx.Y, Radius, Color.Gold);
        }
    }

    public class Rectangle : IShape
    {
        public GLCoordinate Dimensions { get; set; }

        public Rectangle(GLCoordinate dimensions)
        {
            this.Dimensions = dimensions;
        }

        public bool Contains(GameCoordinate point, GameCoordinate location, int angle = 0)
        {
            return (point.X < location.X + Dimensions.X / 2 && point.X > location.X - Dimensions.X / 2 &&
                    point.Y < location.Y + Dimensions.Y / 2 && point.Y > location.Y - Dimensions.Y / 2);
        }

        public void DrawStep(DrawAdapter drawer, GameCoordinate location, int angle)
        {
            GLCoordinate locationx = location.ToGLCoordinate();
            drawer.TraceRectangle(Color.Gold, locationx.X - this.Dimensions.X / 2, -locationx.Y + this.Dimensions.Y / 2, this.Dimensions.X, -this.Dimensions.Y);
        }
    }

    public class Fan : IShape
    {
        public float Length;
        public int Degrees;
        public Fan(float Length, int Degrees)
        {
            this.Length = Length;
            this.Degrees = Degrees;
        }

        public bool Contains(GameCoordinate point, GameCoordinate location, int angle)
        {
            //calculate distance deltas
            var x = Math.Abs(point.X - location.X);
            var y = Math.Abs(point.Y - location.Y);

            //true if point within radius
            var withinRadius = (x * x + y * y < Length * Length);

            //calculate min and max angle
            var minAngle = ((angle - Degrees / 2)); 
            var maxAngle = ((angle + Degrees / 2));

            //calculate point angle relative to location. 
            var pointAngleDeg = ((angleDegrees(location, point)) + 360) %360;

            //true if point angle relative to location is within fan width
            var withinAngle = (pointAngleDeg >= minAngle && pointAngleDeg <= maxAngle);

            //if we within min and max angles and within range.
            return withinRadius && withinAngle;
        }

       
        public void DrawStep(DrawAdapter drawer, GameCoordinate location, int angle)
        {
            var loc = location.ToGLCoordinate();
            drawer.DrawFan(loc.X, loc.Y,  angle, Length, Degrees);
        }

        //todo. this function has been moved to MyMath.cs, but it isnt properly tested. Please confirm if both works.
        private int angleDegrees(GameCoordinate point, GameCoordinate relativeTo)
        {
            if (point == null || relativeTo == null) return 0;
            var deltaX = point.X - relativeTo.X;
            var deltaY = point.Y - relativeTo.Y;
            var alpha = Math.Atan2(deltaY, -deltaX);
            int alphaDeg = (int)((alpha * 180 / Math.PI));
            return alphaDeg;
        }
    }
}