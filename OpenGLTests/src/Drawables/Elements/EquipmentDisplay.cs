﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Entities.Equipment;
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
        private Hero owner;

        public EquipmentDisplay(Hero owner)
        {
            this.owner = owner;
            JsonCoordinateReader.GetEquipmentLocations();
            initBackground();
            initEquipmentSlots();
            this.Visible = false;
            GameState.Drawables.Add(this);
        }

        public void ToggleVisibility()
        {
            this.Visible = !this.Visible;
            foreach (var slot in equipmentSlot)
            {
                slot.Value.Visible = this.Visible;
            }
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

            var leftFootSlot = new EquipmentSlot(owner, new EmptyBoot());
            leftFootSlot.Location = new GLCoordinate(background.Location + slotToLoc[leftFoot]);
            equipmentSlot.Add(new List<EquipmentSlotType>(){ EquipmentSlotType.Feet }, leftFootSlot);

            var rightFootSlot = new EquipmentSlot(owner, new EmptyBoot());
            rightFootSlot.Location = new GLCoordinate(background.Location + slotToLoc[rightFoot]);
            equipmentSlot.Add(new List<EquipmentSlotType>() { EquipmentSlotType.Feet }, rightFootSlot);

            var headSlot = new EquipmentSlot(owner, new EmptyHead());
            headSlot.Location = new GLCoordinate(background.Location + slotToLoc[head]);
            equipmentSlot.Add(new List<EquipmentSlotType>() { EquipmentSlotType.Head }, headSlot);


            foreach (var slot in equipmentSlot) slot.Value.Visible = false;
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
