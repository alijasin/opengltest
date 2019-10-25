using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class ChasingPerson : Hostile
    {
        public ChasingPerson(GameCoordinate location, ICombatable chasing)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);

            ActionPattern = new ChaseEntity(this, chasing);
            ActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_BigDemonRun());
        }
    }
}
