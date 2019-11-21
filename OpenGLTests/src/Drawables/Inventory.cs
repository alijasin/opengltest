using System;
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
        public Unit Owner;
        public Inventory(Unit owner)
        {
            this.Visible = false;
            this.Size = new GLCoordinate(rows*InventorySlot.StandardSize.X + (rows+1)*(fodder), columns * InventorySlot.StandardSize.Y + (columns + 1) * (fodder));
            this.Location = new GLCoordinate(-1 + this.Size.X/2, -1 + this.Size.Y / 2 + 0.2f);
            this.Owner = owner;
            this.Color = Color.Purple;
            this.Depth = 11;

            initSlots();
        }

        private void initSlots()
        {
            int i = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    var islot = new InventorySlot(new Nothing(Owner), this, i);
                    islot.Visible = this.Visible;
                    islot.Location = new GLCoordinate(this.Location.X - this.Size.X / 2 + islot.Size.X / 2, this.Location.Y + this.Size.Y / 2 - islot.Size.Y / 2);
                    islot.Location.X += c * islot.Size.X + fodder * (1 + c);
                    islot.Location.Y += r * -islot.Size.Y - fodder * (1 + r);
                    InventorySlots.Add(islot);
                    i++;
                }
            }
        }

        public bool HasRoom()
        {
            //todo assert if works.
            return InventorySlots.Count(slot => slot.Item is Nothing) > 0;
        }

        /// <summary>
        /// returns index of first empty slot(Nothing). If no room is found returns -1.
        /// </summary>
        /// <returns></returns>
        private int firstEmptySlotIndex()
        {
            var firstNothing = InventorySlots.FirstOrDefault(slot => slot.Item is Nothing);
            if (firstNothing == null) return -1;
            else return firstNothing.IndexSlot;
        }

        public bool Add(Item i)
        {
            //add stack if already in inventory and then return.
            if (i.Stackable && InventorySlots.Any(islot => islot.Item.GetType() == i.GetType()))
            {
                var islot = InventorySlots.Find(isloter => isloter.Item.GetType() == i.GetType());
                islot.Item.Count++;
                return true;
            }

            //if no room return
            var firstEmptyIndex = firstEmptySlotIndex();
            if (firstEmptyIndex == -1) return false;

            InventorySlots[firstEmptyIndex].SetItem(i);
            return true;
        }

        public void Remove(InventorySlot i)
        {
            InventorySlots[i.IndexSlot].SetItem(new Nothing(Owner));
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

        public bool Swap(InventorySlot one, InventorySlot two)
        {
            var temp = one.Item;
            one.SetItem(two.Item);
            two.SetItem(temp);
            return true;
        }
    }
}
