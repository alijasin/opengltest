﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
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


    public abstract class Item : Entity, IDroppable
    {
        public Action OnPickup { get; set; } = () => { };
        public bool Stackable { get; set; } = false;
        public int Count { get; set; } = 1;
        public Rarity Rarity { get; set; } = Rarity.Common;
        public ItemAction Action;
        public bool Consumable { get; set; } = true;
        public bool DestroyOnPickUp { get; set; } = false; //todo: some other way to make this possibly? 
        public SpriteID Icon
        {
            get => Action.Icon;
            set => Action.Icon = value;
        }

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
            this.Action = new NothingAction(owner);
            Action.Icon = SpriteID.missing;
            this.Rarity = Rarity.Common;
        }
    }

    public class Apple : Item
    {
        public Apple(Unit owner)
        {
            Stackable = true;
            this.Action = new TossItemAction(owner, this);
            Action.Icon = SpriteID.item_apple;
            Rarity = Rarity.Rare;
        }
    }

    public class RedPotion : Item
    {
        public RedPotion(Unit owner)
        {
            this.Action = new TurnRedAction(owner, this);
            Action.Icon = SpriteID.item_flask_big_red;
            Rarity = Rarity.Common;
        }
    }


    public class BluePotion : Item
    {
        public BluePotion(Unit owner)
        {
            this.Action = new TurnBlueAction(owner, this);
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
