using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Screens;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public class GameConsole
    {
        public ElementRectangleElement container;

        public GameConsole()
        {
            container = new ElementRectangleElement(18, 2);
            container.Location = new GLCoordinate(0, -1 + container.Size.Y/2);
            container.Color = Color.Red;
            container.Visible = true;
        }

        public void ToggleVisibility()
        {
            container.Visible = !container.Visible;
        }

        public void AddDrawableToBar(Entity d)
        {
            container.AddElement(d);
        }
    }


    public class ElementRectangleElement : RectangleElement, IInteractable
    {
        private int filledSlots = 0;
        private int maxFilledSlot = 0;
        public Dictionary<Element, int> elementSlot = new Dictionary<Element, int>();
        private static GLCoordinate slotSize = new GLCoordinate(0.1f, 0.1f);

        private List<GLCoordinate> slotPosition = new List<GLCoordinate>();

        public ElementRectangleElement(int columns, int rows)
        {
            maxFilledSlot = columns * rows;
            this.Size = new GLCoordinate(slotSize.X*columns, slotSize.Y*rows);
            this.Location = new GLCoordinate(1 - this.Size.X, 1 - this.Size.Y);
            this.Visible = false;
            this.Color = Color.Green;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    slotPosition.Add(new GLCoordinate(c*slotSize.X, r*slotSize.Y));
                }
            }
        }

        public void AddElement(Entity d)
        {
            var e = new DrawableButton(d);
            if (filledSlots >= maxFilledSlot)
            {
                Console.WriteLine("Bar is full, starting to clear from start.");
                filledSlots = 0;
            }

            if (elementSlot.Count > filledSlots)
            {
                if (elementSlot.ElementAt(filledSlots).Key != null)
                {
                    elementSlot.Remove(elementSlot.ElementAt(filledSlots).Key);
                }
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

        public Action OnInteraction { get; set; } = () => { };
    }
}
