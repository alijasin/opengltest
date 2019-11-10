using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class Wizard : Hostile
    {
        public Wizard(GameCoordinate location)
        {
            this.Location = location;
            this.Animation = new Animation(new SpriteSheet_WizardIdle());
            this.Size = new GLCoordinate(0.1f, 0.1f);
        }
    }
}
