﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class DroppedItem : Entity, IClickable
    {
        private Item item;
        EffectGenerator effectGenerator;
        public DroppedItem(Item i, GameCoordinate location)
        {
            item = i;
            this.Animation = new Animation(new SpriteSheet_Icons());
            this.Animation.SetSprite(item.Icon);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Location = location;
            this.Visible = true;

            effectGenerator = new EffectGenerator();
            switch (i.Rarity)
            {
                case Rarity.Common: createColoredEffect(Color.Gray);
                    break;
                case Rarity.Uncommon: createColoredEffect(Color.GreenYellow);
                    break;
                case Rarity.Rare: createColoredEffect(Color.DarkBlue);
                    break;
                case Rarity.Epic: createColoredEffect(Color.Purple);
                    break;
                case Rarity.Legendary: createColoredEffect(Color.DarkOrange);
                    break;
            }

            OnClick = (hero, coordinate) =>
            {
                var added = hero.Inventory.Add(i);

                if (added)
                {
                    this.Dispose();
                    effectGenerator.Dispose();
                }
            };

            GameState.Drawables.Add(this);
        }

        private void createColoredEffect(Color c)
        {
            effectGenerator.CreateCircleEffects(150, this, new GameCoordinate(0.001f, 0.001f), c);
        }

        public Action<Hero, GameCoordinate> OnClick { get; set; }

        public bool Contains(GameCoordinate point)
        {
            return Location.X - Size.X / 2 < point.X && Location.X + Size.X / 2 > point.X &&
                   Location.Y - Size.Y / 2 < point.Y && Location.Y + Size.Y / 2 > point.Y;
        }
    }
}
