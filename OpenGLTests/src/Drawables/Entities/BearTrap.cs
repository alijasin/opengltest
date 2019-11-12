using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class BearTrap : Stuff, IRegion
    {
        private bool isTriggered = false;

        public BearTrap(GameCoordinate location)
        {
            this.Location = location;
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Animation = new Animation(new SpriteSheet_Stuff());
            this.Animation.SetSprite(SpriteID.bear_trap_open);
            this.Animation.IsStatic = true;
            this.Phased = true;
            this.Depth = 0;
            this.Shape = new RangeShape(new Rectangle(this.Size), this);
            //OnClick = coordinate => this.Color = Color.Purple;
        }

        public void Reset()
        {
            isTriggered = false;
            this.Animation.SetSprite(SpriteID.bear_trap_open);
        }

        public Action<GameCoordinate> OnClick { get; set; }

        [JsonIgnore]
        public RangeShape Shape { get; set; }

        public void OnEntered(Unit d)
        {
            //if unit enters this bear trap
            if ((d.GetType().IsSubclassOf(typeof(Unit)) || d is Unit) && isTriggered == false) 
            {
                isTriggered = true;
                this.Animation.SetSprite(SpriteID.bear_trap_closed);
                new DamageStatusEffect(d);
                new RootStatusEffect(d);
            }
        }
    }
}
