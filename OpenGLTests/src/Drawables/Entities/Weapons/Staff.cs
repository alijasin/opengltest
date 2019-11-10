﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities.Weapons
{
    public class Staff : Weapon
    {
        public Staff(Unit owner)
        {
            this.Owner = owner;
            this.Animation = new Animation(new SpriteSheet_Weapon());
            this.Animation.SetSprite(SpriteID.weapon_green_magic_staff);
            this.Animation.IsStatic = true;
            this.Size = new GLCoordinate(0.04f, 0.15f);
            this.InitialSize = new GLCoordinate(0.04f, 0.15f);
            this.LeftFacingRotation = 70;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            drawer.DrawWeapon(this);
        }
    }
}
