using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public abstract class Marker : Entity, IFollowable
    {
        protected Marker(GameCoordinate loc)
        {
            this.Location = loc;
            this.Size = new GLCoordinate(0.070f, 0.070f);
            this.Color = Color.White;
            this.Visible = false;
            this.Animation = new Animation(new SpriteSheet_Icons());
            GameState.Drawables.Add(this);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            if (Visible)
            {
                drawer.DrawSprite(this, DrawAdapter.DrawMode.Centered);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            GameState.Drawables.Remove(this);
        }
    }

    public class ActionMarker : Marker
    {
        public ActionMarker(GameCoordinate loc) : base(loc)
        {
            //this.Color = Color.DarkGoldenrod;
        }
    }

    public class AOEMarker : ActionMarker
    {
        public GLCoordinate aoeSize;
        public AOEMarker(GameCoordinate loc, GLCoordinate aoeSize) : base(loc)
        {
            //this.Color = Color.Red;
            this.aoeSize = aoeSize;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                base.DrawStep(drawer);
                GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);
                drawer.FillCircle(location.X, location.Y, aoeSize, Color.Red);
            }

        }
    }

    public class MoveMarker : ActionMarker
    {
        public MoveMarker(GameCoordinate loc) : base(loc)
        {
            //this.Color = Color.Aqua;
        }
    }
}
