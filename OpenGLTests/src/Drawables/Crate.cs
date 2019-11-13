using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Crate : Stuff, ILeftClickable
    {
        public Crate(GameCoordinate location)
        {
            this.Location = location;
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Animation = new Animation(new SpriteSheet_Stuff());
            this.Animation.SetSprite(SpriteID.crate);
            this.Animation.IsStatic = true;
            OnLeftClick = (hero, coordinate) => this.Color = Color.Purple;
            GameState.Drawables.Add(this);
        }
        
        public Action<Hero, GameCoordinate> OnLeftClick { get; set; }
        public bool LeftClickFilter(Hero hero, GameCoordinate point)
        {
            return Location.X - Size.X / 2 < point.X && Location.X + Size.X / 2 > point.X &&
                   Location.Y - Size.Y / 2 < point.Y && Location.Y + Size.Y / 2 > point.Y;
        }
    }
}
