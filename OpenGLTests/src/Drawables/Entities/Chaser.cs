using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class ChasingPerson : Hostile
    {
        public ChasingPerson(GameCoordinate location)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);

            OutOfCombatActionPattern = new FindAndChaseEntity(this);
            OutOfCombatActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_BigDemonRun());
        }
    }
}
