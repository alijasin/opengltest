using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Button : Rectangle, IInteractable
    {
        public Button()
        {
            this.Size = new GLCoordinate(0.2f, 0.2f);
            OnInteraction += () =>
            {
                Toggle();
            };
        }

        public Action OnInteraction { get; set; }

        public void Toggle()
        {
            Color = Coloring.Opposite(Color);
        }
    }

    abstract class ActionButton : Button
    {
        public GameAction GameAction;
        public static GLCoordinate StandardSize = new GLCoordinate(0.1f, 0.1f);
        protected ActionButton()
        {
            OnInteraction += () =>
            {
                Console.WriteLine("I was clicked " + this);
            };
        }
    }

    class InventorySlot : ActionButton
    {
        public Item Item;

        public InventorySlot(Item i, Inventory inventory)
        {
            this.Item = i;
            this.Color = Color.Yellow;
            this.Size = StandardSize;
            this.Animation = new Animation(new SpriteSheet_Items());
            this.Animation.SetSprite(i.Icon);
            this.Animation.IsStatic = true;
            OnInteraction += () =>
            {
                try
                {
                    inventory.Owner.OutOfCombatActionHandler.Clicked(i.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Tried using an item without an action or something like that: " + e);
                }
            };
        }
    }

    class ActionBarButton : ActionButton
    {

        public static GLCoordinate StandardSize = new GLCoordinate(0.1f, 0.1f);
        public ActionBarButton(GameAction ga, ActionBar inBar)
        {
            this.Size = StandardSize;
            this.Color = Color.Pink;
            GameAction = ga;
            OnInteraction += () =>
            {
                inBar.SetActiveButton(this);
            };
            Toggle();
        }
    }

}
