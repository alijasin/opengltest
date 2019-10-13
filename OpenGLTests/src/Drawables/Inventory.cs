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
        public Item Item;
        public static GLCoordinate StandardSize = new GLCoordinate(0.1f, 0.1f);

        public InventorySlot(Item i)
        {
            this.Item = i;
            this.Color = Color.Yellow;
            this.Size = StandardSize;
            this.Animation = new Animation(new SpriteSheet_Items());
            this.Animation.SetSprite(i.Icon);
            this.Animation.IsStatic = true;
            OnInteraction = () =>
            {
                try
                {
                    i.Action.GetAction().Invoke("ogelibogeli do some cool actioni");
                    Console.WriteLine("Invoked item: " + i.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Tried using an item withuot an action " + e);
                }
            };
        }
    }

    public class Inventory : Rectangle, IClickable
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
            var filledSlots = InventorySlots.Count;
            if (filledSlots < maxSlots)
            {
                InventorySlot islot = new InventorySlot(i);
                islot.Location = new GLCoordinate(this.Location.X - this.Size.X/2 + islot.Size.X/2, this.Location.Y + this.Size.Y/2 - islot.Size.Y/2);
                int col = (filledSlots % columns);
                int row = (filledSlots / rows);
                islot.Location.X += col * islot.Size.X + fodder*(1+col);
                islot.Location.Y += row * -islot.Size.Y - fodder*(1+row);
                InventorySlots.Add(islot);
                GameState.Interactables.Add(islot);
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

        public Action<GameCoordinate> OnClick { get; set; } = coordinate =>
        {
            
        };
    }
}
