using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Entities;

namespace OpenGLTests.src
{
    public class Drawablex
    {
        public List<Drawable> Get => Drawables;
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
