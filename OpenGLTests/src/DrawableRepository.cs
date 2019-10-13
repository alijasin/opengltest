using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Entities;

namespace OpenGLTests.src
{
    public class DrawableRepository
    {
        public List<Drawable> GetAllDrawables => Drawables;
        public List<Hero> GetAllHeroes => Drawables.Where(E => E is Hero).Cast<Hero>().ToList();
        public List<ICombatable> GetAllCombatables => Drawables.Where(E => E is ICombatable).Cast<ICombatable>().ToList();

        public static List<Drawable> Drawables { get; } = new List<Drawable>();

        public void Add(Drawable d)
        {
            Drawables.Add(d);
        }

        public void Remove(Drawable d)
        {
            d.Dispose();
            Drawables.Remove(d);
        }
    }
}
