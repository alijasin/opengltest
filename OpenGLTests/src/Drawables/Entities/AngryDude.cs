using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Entities
{
    class AngryDude : Hostile
    {
        public AngryDude(GameCoordinate Location)
        {
            this.Location = Location;
            this.AggroShape = new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), this);
            this.AggroShape.Visible = true;
            this.Speed = new GameCoordinate(0.01f, 0.01f);
            this.OutOfCombatActionPattern = new StitchedPattern(new MoveAroundAndChill(this), new DebugPattern(this));
            this.OutOfCombatActionPattern.Loop = true;
            Animation = new Animation(new SpriteSheet_BigZombieRun());
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            if (AggroShape != null) AggroShape.DrawStep(drawer);
        }
    }
}
