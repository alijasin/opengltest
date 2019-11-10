using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class Fireball : Effect
    {
        public Fireball()
        {
            this.Size = new GLCoordinate(0.1f, 0.08f);
            this.Location = new GameCoordinate(0, 0.5f);
            this.Animation = new Animation(new SpriteSheet_Fireball());
        }
    }
}
