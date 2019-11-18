using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables.Entities
{
    class FleeingPerson : Hostile
    {
        public FleeingPerson(GameCoordinate location)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            var fleeWithinCircle = new RangeShape(new Circle(new GLCoordinate(0.4f, 0.4f)), this);
            GameState.Drawables.Add(fleeWithinCircle);
            fleeWithinCircle.Visible = true;

            OutOfCombatActionPattern = new FindAndFleeEntity(this, fleeWithinCircle);
            OutOfCombatActionPattern.Loop = true;

            this.Initiative = 111;
            var aggroWithinCircle = new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), this);
            CombatActionPattern = new TailoredPattern(new TurnRedAction(this), new IdleAction(this, 10), new GrowAction(this));
            AggroShape = aggroWithinCircle;

            Animation = new Animation(new SpriteSheet_LizardRun());
        }
    }
}
