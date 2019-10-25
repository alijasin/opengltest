using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;
using Rectangle = OpenGLTests.src.Drawables.Rectangle;

namespace OpenGLTests.src
{
    public class GameConsole
    {
        public ElementRectangle container;

        public GameConsole()
        {
            container = new ElementRectangle(18, 2);
            container.Location = new GLCoordinate(0, -1 + container.Size.Y/2);
            container.Color = Color.Red;
            container.Visible = true;

            for (int i = 0; i < 18 * 2; i++)
            {
                CreateDrawableButton(null);
            }
        }

        public void ToggleVisibility()
        {
            container.Visible = !container.Visible;
        }

        public void CreateDrawableButton(Drawable d)
        {
            container.AddElement(new Button());
        }
    }


    public class ElementRectangle : Rectangle
    {
        private int filledSlots = 0;
        private int maxFilledSlot = 0;
        private Dictionary<Element, int> elementSlot = new Dictionary<Element, int>();
        private static GLCoordinate slotSize = new GLCoordinate(0.1f, 0.1f);

        private List<GLCoordinate> slotPosition = new List<GLCoordinate>();

        public ElementRectangle(int columns, int rows)
        {
            maxFilledSlot = columns * rows;
            this.Size = new GLCoordinate(slotSize.X*columns, slotSize.Y*rows);
            this.Location = new GLCoordinate(1 - this.Size.X, 1 - this.Size.Y);
            this.Visible = false;
            this.Color = Color.Green;

            for (int c = 0; c < columns; c++)
            {
                for(int r = 0; r < rows; r++)
                { 
                    slotPosition.Add(new GLCoordinate(c*slotSize.X, r*slotSize.Y));
                }
            }
        }

        public void AddElement(Element e)
        {
            if (filledSlots >= maxFilledSlot)
            {
                Console.WriteLine("Bar is full!");
                return;
            }

            e.Size = slotSize;
            e.Location = new GLCoordinate(this.Location.X -this.Size.X/2 + slotPosition[filledSlots].X + slotSize.X/2, this.Location.Y - this.Size.Y/2 + slotPosition[filledSlots].Y + slotSize.Y / 2);
            e.Visible = true;
            e.Color = RNG.RandomColor();
            elementSlot.Add(e, filledSlots);

            filledSlots++;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible == false) return;
            base.DrawStep(drawer);
            foreach (var e in elementSlot)
            {
                e.Key.DrawStep(drawer);
            }
        }
    }
}
