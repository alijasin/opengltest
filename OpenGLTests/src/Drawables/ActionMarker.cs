using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public abstract class Marker : Entity
    {
        protected Marker(GameCoordinate loc)
        {
            this.Location = loc;
            this.Size = new GLCoordinate(0.02f, 0.02f);
            this.Visible = false;
            GameState.Drawables.Add(this);
        }
    }
    public class ActionMarker : Marker
    {
        public ActionLine MarkerLine { get; set; }
        public ActionMarker(GameCoordinate loc) : base(loc)
        {
            this.Color = Color.DarkGoldenrod;
            this.MarkerLine = new ActionLine(this.Location);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            if (MarkerLine != null) MarkerLine.DrawStep(drawer);
        }

        public override void Dispose()
        {
            GameState.Drawables.Remove(MarkerLine);
        }

        public void UpdatePositionOfLineAndMarker(GameCoordinate location)
        {
            this.Location = location;
            this.MarkerLine.Location = location;
        }

        public void SetLineOrigin(GameCoordinate gameCoordinate)
        {
            this.MarkerLine.Origin = gameCoordinate;
        }
    }

    public class AOEMarker : ActionMarker
    {
        public GLCoordinate aoeSize;
        public AOEMarker(GameCoordinate loc, GLCoordinate aoeSize) : base(loc)
        {
            this.Color = Color.Red;
            this.aoeSize = aoeSize;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);
            drawer.FillCircle(location.X, location.Y, aoeSize, Color.Red);
        }
    }

    public class MoveMarker : ActionMarker
    {
        public MoveMarker(GameCoordinate loc) : base(loc)
        {
            this.Color = Color.Aqua;
        }
    }
}
