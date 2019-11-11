using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities.Indicators
{
    class WithinCastAreaIndicator : Effect //todo -> indicator
    {
        public WithinCastAreaIndicator(GameCoordinate loc)
        {
            this.Location = loc;
            this.Animation = new Animation(new SpriteSheet_Indicators());
            this.Animation.IsStatic = true;
            this.Animation.SetSprite(SpriteID.indicator_within_area);
            //Console.WriteLine(this.Animation.GetSprite().sid);
            this.Size = new GLCoordinate(0.05f, 0.05f);
            this.Visible = true;
        }
    }
}
