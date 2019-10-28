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

            ActionPattern = new FindAndChaseEntity(this);
            ActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_BigDemonRun());
        }
    }

    class FleeingPerson : Hostile
    {
        public FleeingPerson(GameCoordinate location, ICombatable fleeing)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);

            //ActionPattern = new FleeEntity(this, fleeing);
            ActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_LizardRun());
        }
    }
}
