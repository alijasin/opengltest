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
        private Color initialColor;
        private Color toggleColor;
        public Button()
        {
            initialColor = Color;
            toggleColor = Coloring.Opposite(initialColor);
            this.Size = new GLCoordinate(0.2f, 0.2f);
            OnInteraction += () =>
            {
                //Toggle();
            };
        }

        public Action OnInteraction { get; set; }

        public void Activate()
        {
            Color = toggleColor;
        }

        public void Deactivate()
        {
            Color = initialColor;
        }

        public void Toggle()
        {
            if(Color == initialColor) Color = toggleColor;
            else Color = initialColor;
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

            };
        }

        public void SetGameAction(GameAction ga)
        {

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

                    //inventory.Owner.ActionHandler.OutOfCombatActionHandler.Clicked(i.Action);
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
            GameAction = ga;
            this.Size = StandardSize;
            this.Color = Color.HotPink;
            OnInteraction += () =>
            {
                inBar.SetActiveButton(this);
            };
        }
    }
}
