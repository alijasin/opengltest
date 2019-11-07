using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class Katana : Weapon
    {
        public Katana(Unit owner)
        {
            this.Owner = owner;
            this.Animation = new Animation(new SpriteSheet_Weapon());
            this.Animation.SetSprite(SpriteID.weapon_katana);
            this.Animation.IsStatic = true;
            this.Size = new GLCoordinate(0.04f, 0.2f);
            Rotation = 340;
        }


        public override void DrawStep(DrawAdapter drawer)
        {
            drawer.DrawWeapon(this);
        }
    }
}
