using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Terrain
{

    class BrickWall : Structure
    {
        public BrickWall(GameCoordinate location)
        {
            this.Location = location;
            Animation = new Animation(new SpriteSheet_BrickWall());
            Animation.IsStatic = true;
            Animation.SetSprite(SpriteID.brick_wall);
            Depth = 1;
            this.Size = new GLCoordinate(0.30f, 0.17f);
            this.BoundingBox = new RangeShape(new Rectangle(Size), this);
            this.BoundingBox.Visible = true;
            GameState.Drawables.Add(this);
        }
       
    }

    class BrickGate : Entity, ILeftClickable
    {
        private bool open = false;
        public BrickGate(GameCoordinate location)
        {
            this.Location = location;
            Animation = new Animation(new SpriteSheet_BrickGate());
            Animation.IsStatic = true;
            Animation.SetSprite(SpriteID.brick_gate_closed);
            Size = new GLCoordinate(0.30f, 0.30f);
            Depth = 1;
            GameState.Drawables.Add(this);

            this.OnLeftClick = (hero, p) =>
            {
                this.open = !this.open;
                if (this.open)
                {
                    Animation.SetSprite(SpriteID.brick_gate_open);
                }
                else
                {
                    Animation.SetSprite(SpriteID.brick_gate_closed);
                }
            };
        }

        public Action<Hero, GameCoordinate> OnLeftClick { get; set; }
        public bool LeftClickFilter(Hero hero, GameCoordinate point)
        {
            return Location.X - Size.X / 2 < point.X && Location.X + Size.X / 2 > point.X &&
                   Location.Y - Size.Y / 2 < point.Y && Location.Y + Size.Y / 2 > point.Y;
        }

    }
}
