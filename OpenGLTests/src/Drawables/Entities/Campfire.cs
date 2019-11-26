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

        public Action<Hero, GameCoordinate> OnLeftClick { get; set; }
        public bool LeftClickFilter(Hero hero, GameCoordinate point)
        {
            return ContainsLambdas.ClickedEntityIsPrettyClose(this, hero, point);
        }
    }
}
