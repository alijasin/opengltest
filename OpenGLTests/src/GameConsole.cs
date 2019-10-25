using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using Rectangle = OpenGLTests.src.Drawables.Rectangle;

namespace OpenGLTests.src
{
    public static class GameConsole
    {
        static ElementRectangle container = new ElementRectangle();
        public static void Init()
        {
            Button b = new Button(new GLCoordinate(0.2f, 0.2f));
            b.Color = Color.Yellow;
            b.OnInteraction = () =>
            {
                Console.WriteLine("yayeet");
            };

            container.AddElement(b);
            GameState.Drawables.Add(container);

            Console.WriteLine("Console initialized.");
        }

        public static void ToggleVisibility()
        {
            container.Visible = !container.Visible;
        }
    }

    class ElementRectangle : Rectangle
    {
        private int filledSlots = 0;
        private Dictionary<Element, int> elementSlot = new Dictionary<Element, int>();
        private static GLCoordinate slotSize = new GLCoordinate(0.3f, 0.3f);

        private List<GLCoordinate> slotPosition = new List<GLCoordinate>()
        {
            new GLCoordinate(0, 0),
            new GLCoordinate(slotSize.X, 0),
            new GLCoordinate(0, slotSize.Y),
            new GLCoordinate(slotSize.X, slotSize.Y),
        };

        public ElementRectangle()
        {
            this.Size = new GLCoordinate(slotSize.X*2, slotSize.Y*2);
            this.Location = new GLCoordinate(1 - this.Size.X, 1 - this.Size.Y);
            this.Visible = false;
            this.Color = Color.Green;
        }

        public void AddElement(Element e)
        {
            if (filledSlots >= 4) return;
            e.Size = slotSize;
            e.Location = new GLCoordinate(this.Location.X + slotPosition[filledSlots].X - slotSize.X/2, this.Location.Y + slotPosition[filledSlots].Y + slotSize.Y / 2);
            e.Visible = true;

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
