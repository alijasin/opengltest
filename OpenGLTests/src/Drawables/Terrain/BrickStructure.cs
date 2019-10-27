using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Terrain
{
    class BrickStructure : Entity, IClickable
    {
        private bool open = false;
        public BrickStructure(GameCoordinate location)
        {
            this.Location = location;
            Animation = new Animation(new SpriteSheet_BrickStructure());
            Animation.IsStatic = true;
            Animation.SetSprite(SpriteID.brick_gate_closed);
            Size = new GLCoordinate(0.1f, 0.1f);
            Depth = 1;
            GameState.Drawables.Add(this);

            this.OnClick = p =>
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


        public Action<GameCoordinate> OnClick { get; set; }

        public bool Contains(GameCoordinate clicked)
        {

            return Location.X - Size.X / 2 < clicked.X && Location.X + Size.X / 2 > clicked.X &&
                   Location.Y - Size.Y / 2 < clicked.Y && Location.Y + Size.Y / 2 > clicked.Y;
            //return ContainsLambdas.RectangleContains(this, point);
        }
    }
}
