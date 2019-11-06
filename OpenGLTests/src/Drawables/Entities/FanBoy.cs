using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

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
            this.OutOfCombatActionPattern = new StitchedPattern(new MoveAroundAndChill(this), new DebugPattern(this));
            this.OutOfCombatActionPattern.Loop = true;
            this.CombatActionPattern = new TailoredPattern(new GrowAction(this), new TurnRedAction(this), new InstantTeleport(RNG.RandomPointWithinCircle(new GLCoordinate(0.2f, 0.2f)), this));
            this.CombatActionPattern.Loop = true;
            Animation = new Animation(new SpriteSheet_BigZombieRun());
        }
    }
}
