using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class Bridge : Stuff
    {
        public Bridge(GameCoordinate Location)
        {
            this.Location = Location;
            this.Speed = new GameCoordinate(0, 0);
            this.Size = new GLCoordinate(0.5f, 0.3f);
            var sheet = new SpriteSheet_Bridge();
            this.Animation = new Animation(sheet);
            this.Animation.SetSprite(sheet.GetRandom());
            this.Animation.IsStatic = true;
            this.Visible = true;
            this.Depth = 0;
            this.Phased = true;
        }
    }
}
