using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    class Fire : Effect
    {
        public Fire(GameCoordinate loc, GLCoordinate size)
        {
            this.Size = size;
            this.Location = loc;
            this.Animation = new Animation(new SpriteSheet_Fire());

        }
    }
}
