using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public enum LineType
    {
        Solid,
        Dashed
    }

    public abstract class Line : Drawable
    {
        public virtual GameCoordinate Origin { get; set; }
        public virtual GameCoordinate Terminus { get; set; }
        public LineType LineType = LineType.Solid;

        protected Line(GameCoordinate origin, GameCoordinate terminus, LineType LineType = LineType.Solid)
        {
            this.Origin = origin;
            this.Terminus = terminus;
            this.Color = Color.Green;
            this.LineType = LineType;
            GameState.Drawables.Add(this);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                GLCoordinate GLo = Origin.ToGLCoordinate(GameState.ActiveCamera.Location);
                GLCoordinate GLt = Terminus.ToGLCoordinate(GameState.ActiveCamera.Location);
                drawer.DrawLine(GLo, GLt, Color, LineType);
            }
        }

        public override string ToString()
        {
            return "Origin: " + Origin + "\nTerminus" + Terminus;
        }
    }

    public class ActionLine : Line
    {
        private IFollowable originE;
        public ActionLine(IFollowable originEntity) : base(originEntity.Location, originEntity.Location)
        {
            originE = originEntity;
        }

        public void Set(IFollowable origin, GameCoordinate loc)
        {
            this.originE = origin;
            this.Location = loc;
        }


        public override GameCoordinate Location
        {
            get { return Terminus; }
            set { Terminus = value; }
        }

        public override GameCoordinate Origin
        {
            get
            {
                if (originE != null) return originE.Location;
                else return new GameCoordinate(0, 0);
            }
        }
    }
}
