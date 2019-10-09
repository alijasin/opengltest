using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public class ActionMarker : Entity
    {
        public ActionMarker(GameCoordinate loc)
        {
            this.Location = loc;
            this.Size = new GLCoordinate(0.02f, 0.02f);
            this.Color = Color.DarkGoldenrod;
        }
    }
}
