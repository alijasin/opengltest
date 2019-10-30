using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

/// <summary>
/// these are for inventory
/// </summary>
namespace OpenGLTests.src
{
    public class Nothing : Item
    {
        public Nothing(Unit owner)
        {

        }
    }

    public class Apple : Item
    {
        public Apple(Unit owner)
        {
            this.Action = new TossItemAction(owner, this);
            this.Icon = SpriteID.item_apple;
        }
    }

    public class RedPotion : Item
    {
        public RedPotion(Unit owner)
        {
            this.Action = new TurnRedAction(owner);
            this.Icon = SpriteID.item_flask_big_red;
        }
    }

    public class GrowingPoition : Item
    {
        public GrowingPoition(Unit owner)
        {
            this.Action = new GrowAction(owner);
            this.Icon = SpriteID.item_flask_big_green;
        }
    }
}
