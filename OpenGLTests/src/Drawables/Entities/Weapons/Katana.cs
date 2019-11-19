using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class Katana : Weapon
    {
        public Katana(Unit owner) : base(owner)
        {
            this.Animation = new Animation(new SpriteSheet_Weapon());
            this.Animation.SetSprite(SpriteID.weapon_katana);
            this.Animation.IsStatic = true;
            this.Size = new GLCoordinate(0.04f, 0.2f);
            this.InitialSize = new GLCoordinate(0.04f, 0.2f);
            Rotation = 340;
        }
    }
}
