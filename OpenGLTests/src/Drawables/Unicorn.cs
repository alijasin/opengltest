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
        private ActionPattern ap;
        public Unicorn(GameCoordinate location, Entity chasing)
        {
            this.ap = new ChaseEntity(this, chasing);
            this.Location = location;
            this.Speed = new GameCoordinate(0.01f, 0.01f);
            pg = new ParticleGenerator();
            ap.Loop = true;
        }

        public override void Step()
        {
            base.Step();
            ap.DoAction("ogeli");
            pg.GenerateParticles(10, this.Location);
        }
    }
}
