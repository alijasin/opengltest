using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public static class GameConsole
    {
        public static List<Button> Buttons = new List<Button>();
        public static void Init()
        {
            Button b = new Button(new GLCoordinate(0.2f, 0.2f));
            Buttons.Add(b);
            b.Size = new GLCoordinate(0.3f, 0.3f);
            b.Location = new GLCoordinate(1 - b.Size.X, 1 - b.Size.Y);
            b.Visible = false;
            b.Color = Color.Green;
            GameState.Drawables.Add(b);

            Console.WriteLine("Console initialized.");
        }

        public static void ToggleVisibility()
        {
            foreach (var b in Buttons)
            {
                b.Visible = !b.Visible;
            }
        }
    }
}
