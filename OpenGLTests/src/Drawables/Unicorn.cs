using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    class Unicorn : Hostile
    {
        private ParticleGenerator pg;
        public Unicorn(GameCoordinate location, Entity chasing)
        {
            ActionPattern = new ChaseEntity(this, chasing);
            this.Location = location;
            //this.Speed = new GameCoordinate(0.01f, 0.005f);
            pg = new ParticleGenerator();
            //ActionPattern.Loop = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            pg.GenerateParticles(10, this.Location);
        }
    }
}
