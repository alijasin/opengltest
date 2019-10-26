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

    public interface IShape
    {
        bool Contains(GameCoordinate point, GameCoordinate location);
        void DrawStep(DrawAdapter drawer, GameCoordinate location);
    }

    public class RangeShape : Entity
    {
        public IShape Shape { get; set; }
        public bool IsInfinite { get; set; } = false;
        public IFollowable Following { get; set; }

        public RangeShape(IShape shape, IFollowable following)
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
            return Shape.Contains(point, Location);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (IsInfinite) return;
            if(Visible) Shape.DrawStep(drawer, Location);
        }
    }
    
    public class Circle : IShape
    {
        public GLCoordinate Radius { get; set; }

        public Circle(GLCoordinate radius)
        {
            this.Radius = radius;
        }

        public bool Contains(GameCoordinate point, GameCoordinate location)
        {
            var x = Math.Abs(point.X - location.X);
            var y = Math.Abs(point.Y - location.Y);

            return x * x + y * y < Radius.X * Radius.X; //todo no ellipsis, this is circle
        }

        public void DrawStep(DrawAdapter drawer, GameCoordinate location)
        {
            GLCoordinate locationx = location.ToGLCoordinate();
            drawer.FillCircle(locationx.X, locationx.Y, Radius, Color.Red);
        }
    }
}