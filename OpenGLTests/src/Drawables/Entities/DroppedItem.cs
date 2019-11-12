using System;
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
            createGlowingEffect(i.Rarity);
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

        private void createGlowingEffect(Rarity r)
        {
            effectGenerator.CreateCircleEffects(150, this, new GameCoordinate(0.001f, 0.001f), Coloring.FromRarity(r));
        }

        public Action<Hero, GameCoordinate> OnClick { get; set; }
        public bool ClickFilter(Hero hero, GameCoordinate point)
        {
            //if we are close enough to item
            if (!hero.Location.CloseEnough(hero.Center, this.Size.X*2)) return false;
            //if the click was on the item
            return Location.X - Size.X / 2 < point.X && Location.X + Size.X / 2 > point.X &&
                   Location.Y - Size.Y / 2 < point.Y && Location.Y + Size.Y / 2 > point.Y;
        }
    }
}
