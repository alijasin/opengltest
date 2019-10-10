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
        protected GameCoordinate origin;
        protected GameCoordinate terminus;
        protected LineType LineType = Drawables.LineType.Solid;

        protected Line(GameCoordinate origin, GameCoordinate terminus)
        {
            this.origin = origin;
            this.terminus = terminus;
            this.Color = Color.Green;
        }

        public override void Draw(DrawAdapter drawer)
        {
            GLCoordinate GLo = origin.ToGLCoordinate(GameState.ActiveCamera.Location);
            GLCoordinate GLt = terminus.ToGLCoordinate(GameState.ActiveCamera.Location);
            drawer.DrawLine(GLo, GLt, Color, LineType);
        }
    }

    public class SolidLine : Line
    {
        public SolidLine(GameCoordinate origin, GameCoordinate terminus) : base(origin, terminus)
        {
            this.Color = Color.LawnGreen;
            this.LineType = LineType.Solid;
        }
    }

    public class DashedLine : Line
    {
        public DashedLine(GameCoordinate origin, GameCoordinate terminus) : base(origin, terminus)
        {
            this.Color = Color.DarkGoldenrod;
            this.LineType = LineType.Dashed;
        }
    }
}
