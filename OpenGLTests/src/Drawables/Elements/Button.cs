﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public class Button : Rectangle, IInteractable
    {
        private Color initialColor;
        private Color toggleColor;
        public Button() : this(new GLCoordinate(0.2f, 0.2f))
        {
            GameState.Drawables.RegisterInteractable(this);
        }

        public Button(GLCoordinate size)
        {
            initialColor = Color;
            toggleColor = Coloring.Opposite(initialColor);
            this.Size = size;
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

        public override void Dispose()
        {
            base.Dispose();
            GameState.Drawables.UnRegisterInteractable(this);
        }
    }

    public abstract class ActionButton : Button
    {
        public static GLCoordinate StandardSize = new GLCoordinate(0.1f, 0.1f);
        public GameAction GameAction { get; set; }
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
        public InventorySlot(Item i, Inventory inventory)
        {
            this.Color = Color.Yellow;
            this.Size = StandardSize;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(i.Icon);
            this.Animation.IsStatic = true;
            this.GameAction = i.Action;
            OnInteraction += () =>
            {
                try
                {
                    inventory.Owner.ActionHandler.ActionButtonClicked(this);
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
        public ActionBarButton(Ability sa, ActionBar inBar)
        {
            this.Size = StandardSize;
            this.Color = Color.HotPink;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(sa.Icon);
            this.Animation.IsStatic = true;
            this.GameAction = sa.Action;
            OnInteraction += () =>
            {
                inBar.Owner.ActionHandler.ActionButtonClicked(this);
            };
        }
    }
}