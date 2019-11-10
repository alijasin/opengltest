﻿using System;
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
    interface IClickable
    {
        [JsonIgnore]
        Action<GameCoordinate> OnClick { get; set; }
        bool Contains(GameCoordinate point);
    }

    public interface IShape
    {
        bool Contains(GameCoordinate point, GameCoordinate location, int angle);
        void DrawStep(DrawAdapter drawer, GameCoordinate location, int angle);
    }

    public class RangeShape : Indicator
    {
        public IShape Shape { get; set; }
        public bool IsInfinite { get; set; } = false;
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
            return Shape.Contains(point, Following.Location, Following.FacingAngle);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (IsInfinite) return;
            if (Visible)
            {
                Shape.DrawStep(drawer, Following.Location, Following.FacingAngle);
            }
        }
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
            var x = Math.Abs(point.X - location.X);
            var y = Math.Abs(point.Y - location.Y);

            //true if point within Length
            var withinRadius = (x * x + y * y < Length * Length);

            //calculate min and max angle
            var angle1 = ((angle - Degrees / 2) + 360) % 360; //turn [-180, 180) to [0, 360)
            var angle2 = (angle + Degrees / 2);

            //calculate point angle relative to location. 
            var pointAngleDeg = angleDegrees(location, point);

            //true if point angle relative to location is within fan width
            var withinAngle = ((pointAngleDeg >= angle1 || pointAngleDeg <= Math.Abs(angle1-360)) && (pointAngleDeg <= angle2 || Math.Abs(pointAngleDeg-360) <= angle2));

            return withinRadius && withinAngle;
        }

       
        public void DrawStep(DrawAdapter drawer, GameCoordinate location, int angle)
        {
            var loc = location.ToGLCoordinate();
            drawer.DrawFan(loc.X, loc.Y,  angle, Length, Degrees);
        }

        private int angleDegrees(GameCoordinate point, GameCoordinate relativeTo)
        {
            if (point == null || relativeTo == null) return 0;
            var deltaX = point.X - relativeTo.X;
            var deltaY = point.Y - relativeTo.Y;
            var alpha = Math.Atan2(deltaY, -deltaX);
            int alphaDeg = (int)((alpha * 180 / Math.PI + 360) % 360);
            return alphaDeg;
        }
    }
}