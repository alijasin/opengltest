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
        public ActionLine(GameCoordinate origin) : base(origin, origin)
        {
        }

        public override GameCoordinate Location
        {
            get { return Terminus; }
            set { Terminus = value; }
        }
    }
}
