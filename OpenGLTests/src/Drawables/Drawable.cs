using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenGLTests.src.Drawables
{
    public enum Direction
    {
        Left, Right,
    }
    public abstract class Drawable : ICloneable
    {
        public virtual bool Visible { get; set; }
        public Color Color { get; set; } = Color.White;
        public GLCoordinate Size { get; set; } = new GLCoordinate(0.1f, 0.1f);
        [JsonIgnore]
        public Animation Animation { get; set; }
        public int Depth { get; set; } = 10;
        public Direction Direction { get; set; } = Direction.Right;

        //todo: refactor this. We dont want drawable to  have game location. We want entity to have game location and element ot have gl location.
        private GameCoordinate location;
        [JsonProperty]
        public virtual GameCoordinate Location
        {
            get
            {
                if (this.location == null) return new GameCoordinate(0, 0);
                return this.location;
            }
            set { location = value; }
        }
        public virtual void DrawStep(DrawAdapter drawer)
        {

        }

        public virtual void Dispose()
        {
            GameState.Drawables.Remove(this);
        }

        //create shallow copy
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public abstract class Entity : Drawable
    {
        [JsonProperty]
        public GameCoordinate Speed { get; set; } = new GameCoordinate(0, 0);

        public Entity()
        {
            this.Visible = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            GLCoordinate location = Location.ToGLCoordinate();
            if (Visible)
            {
                if (Animation == null)
                {
                    drawer.FillRectangle(Color, location.X, location.Y, Size.X, Size.Y);
                }
                else
                {
                    drawer.DrawSprite(this);
                }
            }
        }
    }

    public abstract class Element : Drawable
    {
        public GLCoordinate Location { get; set; } = new GLCoordinate(0, 0);

        public Element()
        {
            this.Visible = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                if (Animation == null)
                {
                    drawer.FillRectangle(Color, Location.X, Location.Y, Size.X, Size.Y);
                }
                else
                {
                    drawer.FillRectangle(Color, Location.X, Location.Y, Size.X, Size.Y);
                    drawer.DrawSprite(this);
                }
            }
        }
    }
}
