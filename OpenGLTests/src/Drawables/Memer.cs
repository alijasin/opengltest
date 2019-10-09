using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Entities
{
    class Memer : Entity
    {
        internal Memer()
        {
            Color = Color.Fuchsia;
            this.Location = new GameCoordinate(0, 0);
            this.Size = new GLCoordinate(0.1f, 0.1f);
        }
    }

    class Fever : Entity
    {
        internal Fever()
        {
            Color = Color.Green;
            this.Location = new GameCoordinate(0.4f, 0.4f);
            this.Size = new GLCoordinate(0.2f, 0.2f);
        }
    }
}
