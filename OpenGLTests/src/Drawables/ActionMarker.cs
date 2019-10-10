﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public abstract class Marker : Entity
    {
        private MarkerLine markerLine { get; set; }
        protected Marker(GameCoordinate loc)
        {
            this.Location = loc;
            this.Size = new GLCoordinate(0.02f, 0.02f);
        }

        public void SetMarkerLine(MarkerLine ml)
        {
            this.markerLine = ml;
            GameState.Drawables.Add(ml);
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
            markerLine.Draw(drawer);
        }

        public override void Dispose()
        {
            GameState.Drawables.Remove(markerLine);
        }
    }
    public class ActionMarker : Marker
    {
        public ActionMarker(GameCoordinate loc) : base(loc)
        {
            this.Color = Color.DarkGoldenrod;
        }
    }

    public class AOEMarker : Marker
    {
        private GLCoordinate aoeSize;
        public AOEMarker(GameCoordinate loc, GLCoordinate aoeSize) : base(loc)
        {
            this.Color = Color.Red;
            this.aoeSize = aoeSize;
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
            GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);
            drawer.FillCircle(location.X, location.Y, aoeSize, Color.Red);
        }
    }

    public class MoveMarker : Marker
    {
        public MoveMarker(GameCoordinate loc) : base(loc)
        {
            this.Color = Color.Aqua;
        }
    }
}
