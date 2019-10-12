using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    class InventorySlot : Button
    {
        private Item item;
        public static GLCoordinate StandardSize = new GLCoordinate(0.1f, 0.1f);

        public InventorySlot(Item i)
        {
            this.item = i;
            this.Color = Color.Yellow;
            this.Size = StandardSize;
            OnInteraction = () =>
            {
                try
                {
                    i.Action.GetAction().Invoke("ogelibogeli do some cool actioni");
                    Console.WriteLine("Invoked item: " + i.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Tried using an item withuot an action");
                }
            };
        }
    }

    public class Inventory : Element
    {
        private List<InventorySlot> InventorySlots = new List<InventorySlot>();
        private int rows = 4;
        private int columns = 4;
        private int maxSlots => rows * columns;
        private float fodder = 0.01f;

        public Inventory()
        {
            this.Visible = false;
            this.Location = new GLCoordinate(-0.5f, 0f);
            this.Size = new GLCoordinate(rows*InventorySlot.StandardSize.X + (rows+1)*(fodder), columns * InventorySlot.StandardSize.Y + (columns + 1) * (fodder));
        }

        public bool Add(Item i)
        {
            var count = InventorySlots.Count;
            if (count < maxSlots)
            {
                InventorySlot islot = new InventorySlot(i);
                islot.Location = new GLCoordinate(this.Location.X - this.Size.X/2 + islot.Size.X/2, this.Location.Y + this.Size.Y/2 - islot.Size.Y/2);
                int col = (count % columns);
                int row = (count / rows);
                islot.Location.X += col * islot.Size.X + fodder*(1+col);
                islot.Location.Y += row * -islot.Size.Y - fodder*(1+row);

                InventorySlots.Add(islot);
                GameState.Interactables.Add(islot);
                return true;
            }

            return false;
        }

        public override void Draw(DrawAdapter drawer)
        {
            if (Visible)
            {
                base.Draw(drawer);
                foreach (var islot in InventorySlots)
                {
                    islot.Draw(drawer);
                }
            }

        }
    }
}
