using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Crate : Stuff, IClickable
    {
        public Crate(GameCoordinate location)
        {
            this.Location = location;
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Animation = new Animation(new SpriteSheet_Stuff());
            this.Animation.SetSprite(SpriteID.crate);
            this.Animation.IsStatic = true;
            OnClick = (hero, coordinate) => this.Color = Color.Purple;
            GameState.Drawables.Add(this);
        }
        
        public Action<Hero, GameCoordinate> OnClick { get; set; }

        public bool Contains(GameCoordinate clicked)
        {
            return Location.X - Size.X / 2 < clicked.X && Location.X + Size.X / 2 > clicked.X &&
                Location.Y - Size.Y / 2 < clicked.Y && Location.Y + Size.Y / 2 > clicked.Y;

        }
    }
}
