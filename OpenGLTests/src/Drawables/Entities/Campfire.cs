using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class Campfire : Stuff, ILeftClickable
    {

        public Campfire(GameCoordinate location)
        {
            this.Location = location;
            this.Size = new GLCoordinate(0.15f, 0.15f);
            this.Animation = new Animation(new SpriteSheet_Stuff());
            this.Animation.SetSprite(SpriteID.camp_fire);
            this.Animation.IsStatic = true;
            GameState.Drawables.Add(this);

            OnLeftClick = (hero, coordinate) =>
            {
                this.Animation.SetSprite(SpriteID.extinguish_camp_fire);
            };
        }

        public bool Contains(GameCoordinate clicked)
        {
            return Location.X - Size.X / 2 < clicked.X && Location.X + Size.X / 2 > clicked.X &&
                   Location.Y - Size.Y / 2 < clicked.Y && Location.Y + Size.Y / 2 > clicked.Y;
        }



        public Action<Hero, GameCoordinate> OnLeftClick { get; set; }
        public bool LeftClickFilter(Hero hero, GameCoordinate point)
        {
            return Contains(point);
        }
    }
}
