using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    abstract class Stuff : Entity
    {
        protected Stuff()
        {
            this.Color = Color.White;
        }
    }

    class Crate : Stuff, IClickable
    {
        public Crate(GameCoordinate location)
        {
            this.Location = location;
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Animation = new Animation(new SpriteSheet_Stuff());
            this.Animation.SetSprite(SpriteID.crate);
            this.Animation.IsStatic = true;
            OnClick = coordinate => this.Color = Color.Purple;
            GameState.Drawables.Add(this);
        }
        
        public Action<GameCoordinate> OnClick { get; set; }

        public bool Contains(GameCoordinate point)
        {
            var x = Math.Abs(point.X - Location.X);
            var y = Math.Abs(point.Y - Location.Y);
            //todo move this to somewhere else and fuck yourself.
            GLCoordinate clicked = new GLCoordinate(x * 2 / GibbWindow.WIDTH - 1, (y * 2 / GibbWindow.HEIGHT - 1));
            return this.Location.X - this.Size.X / 2 < clicked.X && this.Location.X + this.Size.X / 2 > clicked.X &&
                   this.Location.Y - this.Size.Y / 2 < clicked.Y && this.Location.Y + this.Size.Y / 2 > clicked.Y;
        }
    }
}
