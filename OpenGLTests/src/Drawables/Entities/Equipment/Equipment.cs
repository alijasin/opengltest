using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities.Equipment
{
    public abstract class EquipmentItem : Item
    {
        public EquipmentSlotType SlotType { get; set; }
        public Unit Owner { get; set; }

        public EquipmentItem(Unit owner)
        {
            this.Owner = owner;
            this.Rarity = Rarity.Common;
            this.Action = new DropEquipmentItem(owner as Hero, this);
            this.Icon = SpriteID.mossy_boot;
            this.Consumable = false;
        }
    }

    public abstract class Weapon : EquipmentItem
    {
        public GLCoordinate InitialSize { get; set; }
        public int Rotation { get; set; } = 0;
        protected int LeftFacingRotation = 120;
        protected int RightFacingRotation = 340;

        public Weapon(Unit owner) : base(owner)
        {

        }

        public override GameCoordinate Location
        {
            get
            {
                if (Owner == null) return new GameCoordinate(0, 0);
                if (!(Owner.ActionHandler.SelectedAction is WeaponAction))
                {
                    if (Owner.Facing == Facing.Right)
                    {
                        Rotation = RightFacingRotation;
                    }
                    else
                    {
                        Rotation = LeftFacingRotation;
                    }
                }

                return new GameCoordinate(Owner.LeftHandLocation.X, Owner.LeftHandLocation.Y);
            }
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            drawer.DrawWeapon(this);
        }

        public Facing GetFacing
        {
            get
            {
                if (Owner == null) return Facing.Right;
                return Owner.Facing;
            }
        }
    }


    public abstract class HeadItem : EquipmentItem
    {
        public HeadItem(Unit owner) : base(owner)
        {
            SlotType = EquipmentSlotType.Head;
        }
    }

    public abstract class BootItem : EquipmentItem
    {
        public BootItem(Unit owner) : base(owner)
        {
            SlotType = EquipmentSlotType.Feet;
            DestroyOnPickUp = true;
        }
    }

    public class EmptyHead : HeadItem
    {
        public EmptyHead(Unit owner) : base(owner)
        {
            Icon = SpriteID.equipment_icon_plate_head;
            DestroyOnPickUp = true;
        }
    }

    public class EmptyBoot : BootItem
    {
        public EmptyBoot(Unit owner) : base(owner)
        {
            Icon = SpriteID.equipment_icon_leather_boots;
        }
    }

    public class MossyBoot : BootItem
    {
        public MossyBoot(Unit owner) : base(owner)
        {
            Icon = SpriteID.mossy_boot;
        }
    }
}
