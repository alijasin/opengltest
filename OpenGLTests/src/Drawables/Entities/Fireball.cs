using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class Fireball : Effect
    {
        public Fireball(GameCoordinate origin)
        {
            this.Size = new GLCoordinate(0.1f, 0.08f);
            this.Location = origin;
            this.Animation = new Animation(new SpriteSheet_Fireball());
            this.Speed = new GameCoordinate(0.01f, 0f);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            //todo: this should be done outside of drawstep.
            this.Location += Speed;
            base.DrawStep(drawer);
        }
    }
}
