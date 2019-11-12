using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Elements
{
    public enum EquipmentSlotType
    {
        Head,
        Hand,
        Feet,
        Leg,
        Chest,
        Trinket,
        Weapon,
    }

    public abstract class EquipmentItem : Entity
    {
        public List<EquipmentSlotType> SlotType { get; set; } = new List<EquipmentSlotType>();
        public SpriteID Icon { get; set; }
        protected EquipmentItem()
        {

        }
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

    public class EquipmentDisplay : Element
    {
        private const string leftFoot = "Left Foot";
        private const string rightFoot = "Right Foot";
        private const string head = "Head";
        private const string leg = "Leg";
        private const string leftHand = "Left Hand";
        private const string rightHand = "Right Hand";

        Dictionary<List<EquipmentSlotType>, ActionButton> equipmentSlot = new Dictionary<List<EquipmentSlotType>, ActionButton>();
        RectangleElement background = new RectangleElement();
        private GLCoordinate backgroundSize = new GLCoordinate(0.6f, 0.6f);

        public EquipmentDisplay()
        {
            JsonCoordinateReader.GetEquipmentLocations();
            initBackground();
            initEquipmentSlots();
            GameState.Drawables.Add(this);
        }

        public void ToggleVisibility()
        {
            this.Visible = !this.Visible;
        }

        private void initBackground()
        {
            background.Size = backgroundSize;
            background.Location = new GLCoordinate(0.5f, 0);
            background.Color = Color.DarkRed;
        }

        private void initEquipmentSlots()
        {
            var slotToLoc = JsonCoordinateReader.GetEquipmentLocations();

            var leftFootSlot = new EquipmentSlot(new EmptyBoot());
            leftFootSlot.Location = new GLCoordinate(background.Location + slotToLoc[leftFoot]);
            equipmentSlot.Add(new List<EquipmentSlotType>(){ EquipmentSlotType.Feet }, leftFootSlot);

            var rightFootSlot = new EquipmentSlot(new EmptyBoot());
            rightFootSlot.Location = new GLCoordinate(background.Location + slotToLoc[rightFoot]);
            equipmentSlot.Add(new List<EquipmentSlotType>() { EquipmentSlotType.Feet }, rightFootSlot);

            var headSlot = new EquipmentSlot(new EmptyHead());
            headSlot.Location = new GLCoordinate(background.Location + slotToLoc[head]);
            equipmentSlot.Add(new List<EquipmentSlotType>() { EquipmentSlotType.Head }, headSlot);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (!this.Visible) return;
            background.DrawStep(drawer);

            foreach (var slot in equipmentSlot)
            {
                slot.Value.DrawStep(drawer);
            }
        }
    }
}
