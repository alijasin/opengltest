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
            this.AvailableActionPoints = 99; //needed not needed? figure it out. todo
            this.Location = location;
            this.Animation = new Animation(new SpriteSheet_WizardIdle());
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Weapon = new Staff(this);

            this.OutOfCombatActionPattern = new TailoredPattern(new IdleAction(this, 20), new TurnAction(this, Facing.Left),
                                            new IdleAction(this, 40), new TurnAction(this, Facing.Right), new FireballAction(this));
            this.OutOfCombatActionPattern.Loop = true;

            this.CombatActionPattern = new TailoredPattern(new IdleAction(this, 40), new FireballAction(this), new IdleAction(this, 5),
                new FireballAction(this), new IdleAction(this, 5), new FireballAction(this), new IdleAction(this, 5), new TeleportAction(new GLCoordinate(0.3f, 0.3f), this));
            this.CombatActionPattern.Loop = true;

            this.AggroShape = new RangeShape(new Circle(new GLCoordinate(0.4f, 0.4f)), this);
  
            this.AggroShape.Visible = true;

            this.SetFacing(Facing.Right);
            OnClick = (hero, coordinate) =>
            {
                if (this.Facing == Facing.Right) SetFacing(Facing.Left);
                else SetFacing(Facing.Right);
            };

            this.LootTable = new LootTable(new LootEntry(new Apple(this), 50), new LootEntry(new GrowingPotion(this), 100));

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


        public Action<Hero, GameCoordinate> OnClick { get; set; }
        public bool ClickFilter(Hero hero, GameCoordinate point)
        {
            return Location.X - Size.X / 2 < point.X && Location.X + Size.X / 2 > point.X &&
                   Location.Y - Size.Y / 2 < point.Y && Location.Y + Size.Y / 2 > point.Y;
        }
    }
}
