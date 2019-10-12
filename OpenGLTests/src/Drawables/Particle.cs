using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Particle : Entity
    {
        private int life;

        public Particle()
        {
            life = RNG.IntegerBetween(5, 50);
            Speed = new GameCoordinate(0.01f, 0.01f);
            Location = new GameCoordinate(0, 0);
            
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
        }
    }
}
