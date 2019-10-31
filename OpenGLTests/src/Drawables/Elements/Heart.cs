using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Elements
{
    class HeartBar : Element
    {
        List<Heart> hearts = new List<Heart>();
        private int numberOfHearts = 0;
        private int filledRows = 0;
        private int slotsPerRow = 8;
        public HeartBar(GLCoordinate Location)
        {
            this.Location = new GLCoordinate(0,0);//Location;
            this.Size = new GLCoordinate(slotsPerRow*(slotSize.X+slotFodder.X) + slotFodder.X *4, -((filledRows+1)*(slotSize.Y+slotFodder.Y)));
            this.Color = Color.Black;
            this.Visible = true;
            this.Depth = 0;
            initialX = this.Location.X - this.Size.X/2 - slotSize.X/2;
            initialY = this.Location.Y + (filledRows-1)*(this.Size.Y/2 + slotSize.Y/2 + slotFodder.Y*2);
            for (int i = 0; i < slotsPerRow+14; i++)
            {
                AddHeart();
            }
        }

        public void SetHearts(int hitpoints)
        {
            hearts.Clear();
            for (int i = 0; i < hitpoints; i++)
            {
                AddHeart();
            }
        }

        private GLCoordinate slotFodder = new GLCoordinate(0.001f, 0.001f);
        private GLCoordinate slotSize = new GLCoordinate(0.05f, 0.05f);
        private float currentXOffset = 0;
        private float currentYOffset = 0;
        private float initialX = 0;
        private float initialY = 0;
        public void AddHeart()
        {
            currentXOffset += slotSize.X + slotFodder.X;
            var loc = new GLCoordinate((initialX + currentXOffset), initialY + currentYOffset);
            Heart h = new Heart(loc);
            hearts.Add(h);
            GameState.Drawables.Add(h);
            numberOfHearts++;
            if (numberOfHearts % slotsPerRow == 0)
            {
                currentXOffset = 0;
                currentYOffset -= slotSize.Y;
                filledRows++;
                this.Size.Y -= slotSize.Y*2;
            }
        }

    }
    class Heart : Element
    {
        public Heart(GLCoordinate gc)
        {
            this.Location = gc;
            this.Size = new GLCoordinate(0.05f, 0.05f);
            this.Visible = true;
            var sheet = new SpriteSheet_Heart();
            this.Animation = new Animation(sheet);
            this.Animation.IsStatic = true;
            this.Animation.SetSprite(SpriteID.heart1);
            Depth = 1;
        }
    }
}
