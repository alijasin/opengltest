using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Terrain;


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
        public List<Unit> GetAllUnits => GetWhere<Unit>(drawable => drawable is Unit);
        public List<Hero> GetAllHeroes => GetWhere<Hero>(drawable => drawable is Hero).ToList();
        public List<ICollidable> GetAllCollidables => GetWhere<ICollidable>(drawable => drawable is ICollidable).ToList();
        public List<IRegion> GetAllRegions => GetWhere<IRegion>(drawable => drawable is IRegion).ToList();
        public List<Effect> GetAllEffects => GetWhere<Effect>(drawable => drawable is Effect).ToList();
        private List<Drawable> toRemove = new List<Drawable>();
        private List<Drawable> toAdd = new List<Drawable>();

        private static object _lock = new object();
        //todo this is digusting and you should change it. Todo!!! High importance low urgency
        //additionaly, i think this just fails sometimes
        private List<T> GetWhere<T>(Func<Drawable, bool> filter)
        {
            lock (_lock)
            {
                for (int i = toRemove.Count-1; i >= 0; i--)
                {
                    if (toRemove.Count > 0)
                    {
                        var success = drawableRepo.Remove(toRemove[i]);
                        if (success) toRemove.RemoveAt(i);
                    }
                }

                if (toAdd.Count > 0)
                {
                    drawableRepo.AddRange(toAdd);
                    toAdd.Clear();
                }
            }

            List<Drawable> temp = new List<Drawable>();

            for (int i = 0; i < drawableRepo.Count; i++)
            {
                var xd = drawableRepo.ElementAt(i);
                if (filter(xd)) temp.Add(xd);
            }
            
            return temp.OrderBy(E => E.Depth).Cast<T>().ToList();
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

        public void Add(Drawable d)
        {
            toAdd.Add(d);
        }

        public void Remove(Drawable d)
        {
            toRemove.Add(d);
        }

        public void Clear(Func<Drawable, bool> ClearFilter)
        {
            toRemove.Clear();
            toAdd.Clear();
            drawableRepo.RemoveAll(e => ClearFilter(e));
        }

        public void ClearExcept(Func<Drawable, bool> KeepFilter = null)
        {
            List<Drawable> toKeep = new List<Drawable>();
            foreach (var d in GetAllDrawables)
            {
                if (KeepFilter(d))
                {
                    toKeep.Add(d);
                }
                else
                {
                    d.Dispose();
                }
            }

            toRemove.Clear();
            toAdd.Clear();
            drawableRepo.Clear();
            drawableRepo.AddRange(toKeep);
        }
    }
}
