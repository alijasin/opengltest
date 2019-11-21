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
    public class EquipmentHandler : Element
    {
        private const string leftFoot = "Left Foot";
        private const string rightFoot = "Right Foot";
        private const string head = "Head";
        private const string leg = "Leg";
        private const string leftHand = "Left Hand";
        private const string rightHand = "Right Hand";

        private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
        private RectangleElement background = new RectangleElement();
        private GLCoordinate backgroundSize = new GLCoordinate(0.6f, 0.6f);
        private Hero owner;

        public EquipmentHandler(Hero owner)
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
            foreach (var slot in equipmentSlots)
            {
                slot.Visible = this.Visible;
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

            var leftFootSlot = new EquipmentSlot(owner, new EmptyBoot(owner));
            leftFootSlot.Location = new GLCoordinate(background.Location + slotToLoc[leftFoot]);
            equipmentSlots.Add(leftFootSlot);

            var rightFootSlot = new EquipmentSlot(owner, new EmptyBoot(owner));
            rightFootSlot.Location = new GLCoordinate(background.Location + slotToLoc[rightFoot]);
            equipmentSlots.Add(rightFootSlot);

            var headSlot = new EquipmentSlot(owner, new EmptyHead(owner));
            headSlot.Location = new GLCoordinate(background.Location + slotToLoc[head]);
            equipmentSlots.Add(headSlot);

            foreach (var slot in equipmentSlots) slot.Visible = false; //todo: double drawing will be done if not this row.
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (!this.Visible) return;
            background.DrawStep(drawer);

            foreach (var slot in equipmentSlots)
            {
                slot.DrawStep(drawer);
            }
        }

        /// <summary>
        /// called from DropItemGameAction.
        /// if we are dropping an item on top of a slot returns the slot
        /// else returns null.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public EquipmentSlot DroppedInSlot(GameCoordinate point)
        {
            var glPoint = point.ToGLCoordinate();

            //lmao i love doing this logic at multiple places. its so much fun!!!! todo
            foreach (var slot in equipmentSlots)
            {
                if (slot.Location.X - slot.Size.X/2 < glPoint.X && slot.Location.X + slot.Size.X/2 > glPoint.X
                 && slot.Location.Y + slot.Size.Y/2 > glPoint.Y && slot.Location.Y - slot.Size.Y/2 < glPoint.Y)
                {
                    return slot;
                }
            }
            return null;
        }

        public void Unequip(EquipmentItem equipmentItem)
        {
            
        }
    }
}