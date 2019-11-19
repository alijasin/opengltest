using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities.Equipment
{
    public abstract class EquipmentItem : Item
    {
        public List<EquipmentSlotType> SlotType { get; set; } = new List<EquipmentSlotType>();

        public EquipmentItem()
        {
            this.Rarity = Rarity.Common;
            this.Action = new LambdaItemAction(o => { return true; }, new Dummy()); //if this aint hack then what is//Todo
            this.Icon = SpriteID.mossy_boot;
        }
    }


    public abstract class Weapon : EquipmentItem
    {
        public GLCoordinate InitialSize { get; set; }
        public int Rotation { get; set; } = 0;
        protected int LeftFacingRotation = 120;
        protected int RightFacingRotation = 340;

        public Unit Owner;
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

        public Facing GetFacing => Owner.Facing;

    }


    public abstract class HeadItem : EquipmentItem
    {
        public HeadItem()
        {
            SlotType.Add(EquipmentSlotType.Head);
        }
    }

    public abstract class BootItem : EquipmentItem
    {
        public BootItem()
        {
            SlotType.Add(EquipmentSlotType.Feet);
        }
    }

    public class EmptyHead : HeadItem
    {
        public EmptyHead()
        {
            Icon = SpriteID.equipment_icon_plate_head;
        }
    }

    public class EmptyBoot : BootItem
    {
        public EmptyBoot()
        {
            Icon = SpriteID.equipment_icon_leather_boots;
        }
    }

    public class MossyBoot : BootItem
    {
        public MossyBoot()
        {
            Icon = SpriteID.mossy_boot;
        }
    }
}
