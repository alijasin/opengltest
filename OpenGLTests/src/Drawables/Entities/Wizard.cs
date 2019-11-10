using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Entities.Weapons;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class Wizard : Hostile, IClickable
    {
        public Wizard(GameCoordinate location)
        {
            this.Location = location;
            this.Animation = new Animation(new SpriteSheet_WizardIdle());
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Weapon = new Staff(this);

            this.SetFacing(Facing.Right);
            OnClick = coordinate =>
            {
                if (this.Facing == Facing.Right) SetFacing(Facing.Left);
                else SetFacing(Facing.Right);
            };

        }

        public override GameCoordinate LeftHandLocation
        {
            get
            {
                if (DoingWeaponAction)
                {
                    return new GameCoordinate(this.Location.X, this.Location.Y);
                }

                if (this.Facing == Facing.Left)
                {
                    return new GameCoordinate(this.Location.X - this.Size.X / 5, this.Location.Y + this.Size.Y / 3f);
                }
                else
                {
                    return new GameCoordinate(this.Location.X + this.Size.X / 2, this.Location.Y - this.Size.Y / 15);
                }
            }
            set { }
        }


        public Action<GameCoordinate> OnClick { get; set; }

        public bool Contains(GameCoordinate clicked)
        {
            return Location.X - Size.X / 2 < clicked.X && Location.X + Size.X / 2 > clicked.X &&
                   Location.Y - Size.Y / 2 < clicked.Y && Location.Y + Size.Y / 2 > clicked.Y;
        }
    }
}
