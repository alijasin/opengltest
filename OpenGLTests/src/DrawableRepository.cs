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
    //Todo: this whole class is shit and you should feel bad.
    public class DrawableRepository
    {
        private static List<Drawable> drawableRepo { get; } = new List<Drawable>();
        private static List<IInteractable> interactableRepo { get; } = new List<IInteractable>();


        public List<Drawable> GetAllDrawables => drawableRepo;
        public List<Entity> GetAllEntities => drawableRepo.Where(e => e is Entity).Cast<Entity>().ToList();
        public List<Element> GetAllElements => drawableRepo.Where(e => e is Element).Cast<Element>().ToList();
        public List<IInteractable> GetAllInteractables => interactableRepo;

        public List<Hero> GetAllHeroes
        {
            get
            {
                try
                {
                    var xd = drawableRepo.Where(E => E is Hero).Cast<Hero>().ToList();
                    return xd;
                    /*var size = drawableRepo.Count(E => E is ICombatable);
                    List<ICombatable> combtables = new List<ICombatable>(size);
                    return drawableRepo.Where(E => E is ICombatable).Cast<ICombatable>().Take(size).ToList();*/
                }
                catch (Exception e)
                {
                    return new List<Hero>();
                }

            }
        }

        public List<ICombatable> GetAllCombatables 
        {
            get
            {
                try
                {
                    /*var xd = drawableRepo.Where(E => E is ICombatable).Cast<ICombatable>().ToList();
                    return xd;*/
                    var size = drawableRepo.Count(E => E is ICombatable);
                    List<ICombatable> combtables = new List<ICombatable>(size);
                    return drawableRepo.Where(E => E is ICombatable).Cast<ICombatable>().Take(size).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("fuck");
                    Console.WriteLine(e);
                    return new List<ICombatable>();
                }
            }
        }

        /// <summary>
        /// Register an interactable so that it can be interacted with. This function will not register the interactable's ondraw function to be called automatically.
        /// </summary>
        /// <param name="i"></param>
        public void RegisterInteractable(IInteractable i)
        {
            interactableRepo.Add(i);
        }

        /// <summary>
        /// Adds a drawable to the drawable repo. All drawable's onDraw function will be called on each render automatically.
        /// </summary>
        /// <param name="d"></param>
        public void Add(Drawable d)
        {
            drawableRepo.Add(d);
        }

        public void Remove(Drawable d)
        {
            drawableRepo.Remove(d);
        }
    }
}
