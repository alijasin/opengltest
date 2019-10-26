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

        private static List<Drawable> drawableRepo { get; } = new List<Drawable>();
        public List<Drawable> GetAllDrawables => GetWhere<Drawable>(drawable => true);
        public List<Entity> GetAllEntities => GetWhere<Entity>(drawable => drawable is Entity);
        public List<Element> GetAllElements => GetWhere<Element>(drawable => drawable is Element);
        public List<ICombatable> GetAllCombatables => GetWhere<ICombatable>(drawable => drawable is ICombatable);
        public List<Hero> GetAllHeroes => drawableRepo.Where(E => E is Hero).Cast<Hero>().ToList();

        private List<Drawable> toRemove = new List<Drawable>();

        private List<T> GetWhere<T>(Func<Drawable, bool> filter)
        {
            object lockObj = 1;
            lock (lockObj)
            {
                foreach (var rem in toRemove.ToList())
                {
                    bool success = drawableRepo.Remove(rem);
                    if (success) toRemove.Remove(rem);
                }

                drawableRepo.AddRange(toAdd);
                toAdd.Clear();
                return drawableRepo.Where(filter).Cast<T>().ToList();
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

        public void UnRegisterInteractable(IInteractable i)
        {
            interactableRepo.Remove(i);
        }

        private List<Drawable> toAdd = new List<Drawable>();

        public void Add(Drawable d)
        {
            //drawableRepo.Add(d);
            toAdd.Add(d);
        }

        public void Remove(Drawable d)
        {
            toRemove.Add(d);
            //drawableRepo.Remove(d);
            //drawableRepo.TryTake(d);
        }
    }
}
