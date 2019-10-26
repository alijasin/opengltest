using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Terrain
{
    public class Floor : Entity
    {
        public Floor(GameCoordinate location)
        {
            this.Location = location;
            var sheet = new SpriteSheet_Floor();
            Animation = new Animation(sheet);
            Animation.IsStatic = true;
            Animation.SetSprite(sheet.GetRandom());
            Size = new GLCoordinate(0.1f, 0.1f);
            Depth = 0;
        }
    }
}
