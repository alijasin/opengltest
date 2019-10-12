using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public abstract class Item
    {
        public ItemAction Action;
    }

    public class Nothing : Item
    {

    }

    public class RedPotion : Item
    {
        public RedPotion(Entity owner)
        {
            this.Action = new TurnRedAction(owner);
        }
    }

    public class GrowingPoition : Item
    {
        public GrowingPoition(Entity owner)
        {
            this.Action = new GrowAction(owner);
        }
    }
}
