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
                drawer.DrawSprite(this);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
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
        public RangeShape aoeShape;
        public AOEMarker(GameCoordinate loc, RangeShape aoeShape) : base(loc)
        {
            this.Location = loc;
            this.aoeShape = aoeShape;
            aoeShape.Following = this;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                base.DrawStep(drawer);
                aoeShape.DrawStep(drawer);
            }
        }

        public override bool Visible
        {
            set
            {
                base.Visible = value;
                if(aoeShape != null) aoeShape.Visible = value;
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
