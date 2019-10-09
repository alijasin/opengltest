using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Entities;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class GameState
    {
        public Hero Hero { get; set; }
        public static Camera Cam { get; set; }

        public static List<Drawable> Drawables { get; set; } = new List<Drawable>()
        {
            new Memer(),
            new Fever(),
        };

        public GameState()
        {
            Hero = new Hero();
            Drawables.Add(Hero);
            Memer m = new Memer();
            m.Location = new GameCoordinate(3f, 0);
            Drawables.Add(m);

            Cam = new FollowCamera(Hero);
        }

        public void Step()
        {
            foreach (var e in Drawables.Where(d => d is Entity))
            {
                e.Step();
            }
        }

        public static void CombatStep()
        {
            var drawables = Drawables.ToList();
            foreach (var e in drawables)
            {
                e.CombatStep();
            }
        }

        public static void AddDrawable(Drawable d)
        {
            Drawables.Add(d);
        }
    }
}
