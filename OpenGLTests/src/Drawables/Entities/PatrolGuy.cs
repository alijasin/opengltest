using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class PatrolGuy : Hostile
    {
        public PatrolGuy(GameCoordinate location)
        {
            this.Speed = new GameCoordinate(0.01f, 0.005f);
            this.Location = location;

            ActionPattern = new NeverEndingPatrol(this, new GameCoordinate(0.2f, 0));
            ActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_OgreRun());
        }
    }
}
