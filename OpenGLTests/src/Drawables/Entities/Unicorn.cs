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
        public Unicorn(GameCoordinate location, Unit chasing)
        {
            OutOfCombatActionPattern = new FindAndChaseEntity(this);

            this.Location = location;
            //this.Speed = new GameCoordinate(0.01f, 0.005f);
            pg = new ParticleGenerator(80, this.Location);
            //OutOfCombatActionPattern.Loop = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (!Visible) return;
                base.DrawStep(drawer);
             pg.Draw(drawer);
        }
    }
}
