using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class Rock : Stuff
    {
        public Rock(GameCoordinate Location)
        {
            this.Location = Location;
            this.Speed = new GameCoordinate(0.0f, 0.0f);
            this.Size = new GLCoordinate(0.2f, 0.2f);
            var sheet = new SpriteSheet_Rocks();
            this.Animation = new Animation(sheet);
            this.Animation.SetSprite(sheet.GetRandom());
            this.Animation.IsStatic = true;
            this.Visible = true;
            GameState.Drawables.Add(this);
        }
    }
}
