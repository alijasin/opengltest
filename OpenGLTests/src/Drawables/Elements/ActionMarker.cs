using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLTests.src.Drawables
{
    public abstract class Marker : Indicator
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
                drawer.DrawEntity(this);
            }
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
        public RangeShape AOEShape;
        public AOEMarker(GameCoordinate loc, RangeShape aoeShape) : base(loc)
        {
            this.Location = loc;
            this.AOEShape = aoeShape;
            aoeShape.Following = this;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                base.DrawStep(drawer);
                AOEShape.DrawStep(drawer);
            }
        }

        public override bool Visible
        {
            set
            {
                base.Visible = value;
                if(AOEShape != null) AOEShape.Visible = value;
            }
        }
    }

    public class MoveMarker : ActionMarker
    {
        public MoveMarker(GameCoordinate loc) : base(loc)
        {

        }
    }
}
