using System;
using System.Collections.Concurrent;
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
        private static List<IInteractable> interactableRepo { get; } = new List<IInteractable>();
        public List<IInteractable> GetAllInteractables => interactableRepo;

        private static BlockingCollection<Drawable> drawableRepo { get; } = new BlockingCollection<Drawable>();
        public List<Drawable> GetAllDrawables => drawableRepo.ToList();
        public List<Entity> GetAllEntities => drawableRepo.Where(e => e is Entity).Cast<Entity>().ToList();
        public List<Element> GetAllElements => drawableRepo.Where(e => e is Element).Cast<Element>().ToList();
        public List<Hero> GetAllHeroes => drawableRepo.Where(E => E is Hero).Cast<Hero>().ToList();
        public List<ICombatable> GetAllCombatables => drawableRepo.Where(E => E is ICombatable).Cast<ICombatable>().ToList();

        /// <summary>
        /// Register an interactable so that it can be interacted with. This function will not register the interactable's ondraw function to be called automatically.
        /// </summary>
        /// <param name="i"></param>
        public void RegisterInteractable(IInteractable i)
        {
            interactableRepo.Add(i);
        }

        public void UnRegisterInteractable(IInteractable i)
        {
            interactableRepo.Remove(i);
        }

        private List<Drawable> toAdd = new List<Drawable>();
        public void Add(Drawable d)
        {
            drawableRepo.TryAdd(d, 1);
            //toAdd.Add(d);
        }
        private List<Drawable> toRemove = new List<Drawable>();
        public void Remove(Drawable d)
        {
            toRemove.Add(d);
            //drawableRepo.Remove(d);
        }

        public void Update()
        {
            /*drawableRepo.AddRange(toAdd);
            toAdd.Clear();*/

            foreach (var tr in toRemove)
            {
                //Drawable res;
                //drawableRepo.TryTake(out res);
            }
        }
    }
}
