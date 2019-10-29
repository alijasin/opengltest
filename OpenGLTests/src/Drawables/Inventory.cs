using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public class Inventory : RectangleElement, IClickable
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
            this.Location = new GLCoordinate(-0.5f, 0f);
            this.Size = new GLCoordinate(rows*InventorySlot.StandardSize.X + (rows+1)*(fodder), columns * InventorySlot.StandardSize.Y + (columns + 1) * (fodder));
            this.Owner = owner;
        }

        public bool Add(Item i)
        {
            var filledSlots = InventorySlots.Count;
            if (filledSlots < maxSlots)
            {
                InventorySlot islot = new InventorySlot(i, this);
                islot.Visible = false;
                islot.Location = new GLCoordinate(this.Location.X - this.Size.X/2 , this.Location.Y + this.Size.Y/2 );
                int col = (filledSlots % columns);
                int row = (filledSlots / rows);
                islot.Location.X += col * islot.Size.X + fodder*(1+col);
                islot.Location.Y += row * -islot.Size.Y - fodder*(1+row);
                InventorySlots.Add(islot);
                return true;
            }

            return false;
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

        public Action<GameCoordinate> OnClick { get; set; } = coordinate =>
        {
            
        };
    }
}
