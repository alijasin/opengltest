using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

/// <summary>
/// these are for inventory
/// </summary>
namespace OpenGLTests.src
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public abstract class Item : Entity
    {
        public Rarity Rarity { get; set; } = Rarity.Common;
        public ItemAction Action;
        public SpriteID Icon => Action.Icon;

        public override void Dispose()
        {
            Action.Dispose();
            base.Dispose();
        }
    }

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
            Action.Icon = SpriteID.item_apple;
            Rarity = Rarity.Rare;
        }
    }

    public class RedPotion : Item
    {
        public RedPotion(Unit owner)
        {
            this.Action = new TurnRedAction(owner);
            Action.Icon = SpriteID.item_flask_big_red;
            Rarity = Rarity.Common;
        }
    }

    public class GrowingPotion : Item
    {
        public GrowingPotion(Unit owner)
        {
            this.Action = new GrowAction(owner);
            Action.Icon = SpriteID.item_flask_big_green;
            Rarity = Rarity.Common;
        }
    }
}
