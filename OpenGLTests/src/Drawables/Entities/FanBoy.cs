using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class FanBoy : Hostile
    {
        public FanBoy(GameCoordinate Location)
        {
            this.Location = Location;
            this.AggroShape = new RangeShape(new Fan(0.4f, 80), this);
            this.AggroShape.Visible = true;
            this.Speed = new GameCoordinate(0.005f, 0.005f);
            this.ActionPattern = new CustomPattern(new MoveAroundAndChill(this), new DebugPattern(this));
            this.ActionPattern.Loop = true;
            Animation = new Animation(new SpriteSheet_BigZombieRun());
        }
    }
}
