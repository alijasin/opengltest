using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class Dummy : Unit
    {
        public Dummy()
        {
            this.Visible = false;
            this.Location = new GameCoordinate(-9999, -9999);
            this.Size = new GLCoordinate(0, 0);
            this.Speed = new GameCoordinate(0,0);
        }

        public override void OnAggro(Unit aggroed)
        {

        }

        public override void CombatStep(Fight fight)
        {

        }

        public override void OutOfCombatStep()
        {

        }
    }
}
