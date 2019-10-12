using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public abstract class Drawable
    {
        public bool Visible { get; set; } = true;
        public Color Color { get; set; } = Color.Fuchsia;
        public GLCoordinate Size { get; set; } = new GLCoordinate(0.1f, 0.1f);
        

        public virtual void Draw(DrawAdapter drawer)
        {
            
        }

        public virtual void DrawStep()
        {

        }

        public virtual void Dispose()
        {

        }
    }

    public abstract class Entity : Drawable
    {
        public GameAction CurrentlyDoing { get; set; }
        //public virtual GameCoordinate Location { get; set; } = new GameCoordinate(0, 0);
        public GameCoordinate Speed { get; set; } = new GameCoordinate(0, 0);
        public override void Draw(DrawAdapter drawer)
        {
            GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);
            if (Visible) drawer.FillRectangle(Color, location.X, location.Y, Size.X, Size.Y);
        }

        private GameCoordinate location;
        public virtual GameCoordinate Location
        {
            get
            {
                if (this.location == null) return new GameCoordinate(0, 0);
                return this.location;
            }
            set { location = value; }
        }

        public virtual void CombatStep()
        {

        }

        public virtual void OutOfCombatStep()
        {

        }
    }

    public abstract class Element : Drawable
    {
        public virtual GLCoordinate Location { get; set; } = new GLCoordinate(0, 0);

        public override void Draw(DrawAdapter drawer)
        {
            if (Visible) drawer.FillRectangle(Color, Location.X, Location.Y, Size.X, Size.Y);
        }
    }
}
