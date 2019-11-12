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
        public EquipmentSlotType SlotType { get; set; }
        public SpriteID Icon { get; set; }
        protected EquipmentItem()
        {

        }
    }

    public abstract class BootItem : EquipmentItem
    {
        public BootItem()
        {
            SlotType = EquipmentSlotType.Feet;
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

    public class Equipment : Element
    {
        Dictionary<EquipmentSlotType, ActionButton> equipmentSlot = new Dictionary<EquipmentSlotType, ActionButton>();
        RectangleElement background = new RectangleElement();
        private GLCoordinate backgroundSize = new GLCoordinate(0.5f, 0.5f);


        public Equipment()
        {
            initBackground();
            addEquipmentSlot();
        }

        private void initBackground()
        {
            background.Size = backgroundSize;
            background.Location = new GLCoordinate(0, 0);
            background.Color = Color.DarkRed;
            GameState.Drawables.Add(background);
        }

        private void addEquipmentSlot()
        {
            EmptyBoot mb = new EmptyBoot();
            var slot = new EquipmentSlot(mb);
            mb.Location = new GameCoordinate(0, 0);
            equipmentSlot.Add(mb.SlotType, slot);
            GameState.Drawables.Add(slot);
        }
    }
}
