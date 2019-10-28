using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class FleeingPerson : Hostile
    {
        public FleeingPerson(GameCoordinate location)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);

            ActionPattern = new FindAndFleeEntity(this);
            ActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_LizardRun());
        }
    }
}
