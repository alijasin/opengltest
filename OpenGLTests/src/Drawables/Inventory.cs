﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public class Inventory : RectangleElement, ILeftClickable
    {
        private List<InventorySlot> InventorySlots = new List<InventorySlot>();
        private int rows = 4;
        private int columns = 4;
        private int maxSlots => rows * columns;
        private float fodder = 0.01f;
        public IActionCapable Owner;
        public Inventory(IActionCapable owner)
        {
            this.Visible = false;
            this.Size = new GLCoordinate(rows*InventorySlot.StandardSize.X + (rows+1)*(fodder), columns * InventorySlot.StandardSize.Y + (columns + 1) * (fodder));
            this.Location = new GLCoordinate(-1 + this.Size.X/2, -1 + this.Size.Y / 2 + 0.2f);
            this.Owner = owner;
            this.Color = Color.Purple;
            this.Depth = 11;
        }

        public bool HasRoom()
        {
            //todo: tests
            return InventorySlots.Count < maxSlots;
        }

        public bool Add(Item i)
        {
            if (i.Stackable && InventorySlots.Any(islot => islot.Item.GetType() == i.GetType()))
            {
                var islot = InventorySlots.Find(isloter => isloter.Item.GetType() == i.GetType());
                islot.Item.Count++;
                return true;
            }

            var filledSlots = InventorySlots.Count;
            if (filledSlots < maxSlots)
            {
                InventorySlot islot = new InventorySlot(i, this);
                islot.Visible = this.Visible;
                islot.Location = new GLCoordinate(this.Location.X - this.Size.X/2 + islot.Size.X / 2, this.Location.Y + this.Size.Y/2 - islot.Size.Y/2);
                int col = (filledSlots % columns);
                int row = (filledSlots / rows);
                islot.Location.X += col * islot.Size.X + fodder*(1+col);
                islot.Location.Y += row * -islot.Size.Y - fodder*(1+row);
                InventorySlots.Add(islot);
                return true;
            }

            return false;
        }

        public void Remove(InventorySlot i)
        {
            InventorySlots.Remove(i);
            i.Dispose();
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible)
            {
                base.DrawStep(drawer);
                foreach (var islot in InventorySlots)
                {
                    islot.DrawStep(drawer);
                }
            }
        }

        public override bool Visible
        {
            set
            {
                base.Visible = value;
                foreach (var islot in InventorySlots)
                {
                    islot.Visible = value;
                }
            }
        }

        public Action<Hero, GameCoordinate> OnLeftClick { get; set; } = (hero, coordinate) =>
        {
            
        };

        public bool LeftClickFilter(Hero hero, GameCoordinate point)
        {
            return false;
        }
    }
}
