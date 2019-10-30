using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class House : Stuff
    {
        public House(GameCoordinate Location)
        {
            this.Location = Location;
            this.Speed = new GameCoordinate(0, 0);
            this.Size = new GLCoordinate(0.7f, 0.7f);
            var sheet = new SpriteSheet_WoodenHouse();
            this.Animation = new Animation(sheet);
            this.Animation.SetSprite(sheet.GetRandom());
            this.Animation.IsStatic = true;
            this.Visible = true;
            GameState.Drawables.Add(this);
        }
    }
}
