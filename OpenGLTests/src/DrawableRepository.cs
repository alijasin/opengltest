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
        private static List<Drawable> drawableRepo { get; } = new List<Drawable>();
        public List<Drawable> GetAllDrawables => drawableRepo;
        public List<Hero> GetAllHeroes => drawableRepo.Where(E => E is Hero).Cast<Hero>().ToList();
        public List<ICombatable> GetAllCombatables 
        {
            
            get
            {
                try
                {
                    var xd = drawableRepo.Where(E => E is ICombatable).Cast<ICombatable>().ToList();
                    return xd;
                    /*var size = drawableRepo.Count(E => E is ICombatable);
                    List<ICombatable> combtables = new List<ICombatable>(size);
                    return drawableRepo.Where(E => E is ICombatable).Cast<ICombatable>().Take(size).ToList();*/
                }
                catch (Exception e)
                {
                    return new List<ICombatable>();
                }
            }
        }


        public void Add(Drawable d)
        {
            drawableRepo.Add(d);
        }

        public void Remove(Drawable d)
        {
            d.Dispose();
            drawableRepo.Remove(d);
        }
    }
}
