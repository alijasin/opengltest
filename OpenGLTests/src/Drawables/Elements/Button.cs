using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public class Button : RectangleElement, IInteractable
    {
        public Color BorderColor = System.Drawing.Color.Black;
        public Color BackColor = Color.Firebrick;
        public virtual bool Enabled { get; set; } = true;

        protected bool HasBorder = true;
        protected bool HasBackground = true;
        public Button() : this(new GLCoordinate(0.2f, 0.2f))
        {
            GameState.Drawables.RegisterInteractable(this);
        }

        public Button(GLCoordinate size)
        {
            this.Size = size;
            OnInteraction += () =>
            {
                //Toggle();
            };
        }

        public Action OnInteraction { get; set; }

        public override void Dispose()
        {
            base.Dispose();
            GameState.Drawables.UnRegisterInteractable(this);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (!Visible) return;
            if(HasBackground) drawer.FillRectangle(this.BackColor, this.Location.X, this.Location.Y, this.Size.X, this.Size.Y);
            if(HasBorder)drawer.TraceRectangle(this.BorderColor, this.Location.X - this.Size.X / 2, -this.Location.Y + this.Size.Y / 2, this.Size.X, -this.Size.Y);
            base.DrawStep(drawer);
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
                try
                {
                    GameAction.OnSelected();
                }
                catch (Exception e)
                {
                    Console.WriteLine("No action set");
                }

            };
        }

        public void SetGameAction(GameAction ga)
        {

        }
    }

    public class EquipmentSlot : ActionButton, IRightClickable
    {
        private Hero owner;
        public EquipmentSlot(Hero owner, EquipmentItem ei)
        {
            this.owner = owner;
            this.Color = Color.White;
            this.Size = StandardSize;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(ei.Icon);
            this.Animation.IsStatic = true;
            this.GameAction = new DropEquipmentItem(owner, ei);
            this.BackColor = Coloring.FromRarity(ei.Rarity);
            OnInteraction += () =>
            {
                try
                {
                    //owner.ActionHandler.ActionButtonActivated(this);
                }
                catch (Exception e)
                {

                }
            };

            OnRightClick = (hero, coordinate) =>
            {
                //if(owner.ActionHandler.)
                owner.ActionHandler.ActionButtonActivated(this);
                Player.Cursor.SetIcon(ei.Icon);
            };
        }

        public Action<Hero, GameCoordinate> OnRightClick { get; set; } 
        public bool RightClickFilter(Hero hero, GameCoordinate point)
        {
            return Contains(point);
        }

        public void Unequip()
        {
            owner.EquipmentDisplay.Unequip(this);
        }
    }

    public class InventorySlot : ActionButton, IRightClickable
    {
        public Item Item { get; set; }
        public int IndexSlot { get; set; }
        private Inventory inventory;
    
        public InventorySlot(Item i, Inventory inventory, int indexSlot)
        {
            this.IndexSlot = indexSlot;
            this.Item = i;
            this.inventory = inventory;
            this.BackColor = Coloring.FromRarity(i.Rarity);
            this.Size = StandardSize;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(i.Icon);
            this.Animation.IsStatic = true;
            this.GameAction = i.Action;
            OnInteraction += () =>
            {
                try
                {
                    //inventory.Owner.ActionHandler.ActionButtonActivated(this);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Tried using an item without an action or something like that: " + e);
                }
            };

            OnRightClick = (hero, coordinate) =>
            {
                if(inventory.Owner.ActionHandler.CurrentButtonSelected != null) Swap(inventory.Owner.ActionHandler.CurrentButtonSelected);
                inventory.Owner.ActionHandler.ActionButtonActivated(this);
                Player.Cursor.SetIcon(this.Item.Icon);
            };
        }

        public void Swap(ActionButton other)
        {
            if(other is InventorySlot iother) inventory.Swap(this, iother);
        }

        public void Consume()
        {
            //if (!(Item.Consumable)) return;
            this.Item.Count--;

            if (this.Item.Count < 1)
            {
                inventory.Remove(this);
            }
        }

        public Action<Hero, GameCoordinate> OnRightClick { get; set; }
        public bool RightClickFilter(Hero hero, GameCoordinate point)
        {
            return Contains(point);
        }

        //todo use this in the constructor
        public void SetItem(Item item)
        {
            this.Item = item;
            this.BackColor = Coloring.FromRarity(item.Rarity);
            this.Animation.SetSprite(item.Icon);
            this.GameAction = item.Action;
        }
    }

    class ActionBarButton : ActionButton
    {
        public static GLCoordinate StandardSize = new GLCoordinate(0.1f, 0.1f);
        public ActionBarButton(Ability sa, ActionBar inBar)
        {
            this.Size = StandardSize;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(sa.Icon);
            this.Animation.IsStatic = true;
            this.GameAction = sa.Action;
            OnInteraction += () =>
            {
                inBar.Owner.ActionHandler.ActionButtonActivated(this);
            };
        }
    }

    class CombatTurnConfirmationButton : Button
    {
        public override bool Enabled
        {
            set
            {
                base.Enabled = value;
                if (Enabled) this.Color = Color.Green;
                else this.Color = Color.Red;
            }
        }

        public CombatTurnConfirmationButton(GLCoordinate Loc)
        {
            this.Visible = false; //assume we not in combat when we starting
            this.Color = Color.Green;
            this.Location = Loc;
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Animation = new Animation(new SpriteSheet_Weapon());
            this.Animation.SetSprite( SpriteID.weapon_golden_sword);
            this.Animation.IsStatic = true;
        }
    }

    public class InventoryButton : Button
    {
        private Inventory inventory;
        public InventoryButton(GLCoordinate relativeLoc, Inventory i)
        {
            this.Size = new GLCoordinate(0.2f, 0.2f);
            this.Location = new GLCoordinate(relativeLoc.X + this.Size.X/2, relativeLoc.Y + this.Size.Y / 2);
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(SpriteID.bag_closed);
            this.Animation.IsStatic = true;
            this.inventory = i;

            this.OnInteraction = () =>
            {
                ShowInventory();
            };
        }

        public void ShowInventory()
        {
            if (this.Animation.GetSprite().sid == SpriteID.bag_closed) this.Animation.SetSprite(SpriteID.bag_open);
            else this.Animation.SetSprite(SpriteID.bag_closed);

            inventory.Visible = !inventory.Visible;
        }
    }
}
