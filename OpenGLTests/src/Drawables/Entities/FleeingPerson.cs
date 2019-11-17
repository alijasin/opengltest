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

            var chasingAggroShape = new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), this);
            GameState.Drawables.Add(chasingAggroShape);
            chasingAggroShape.Visible = true;

            OutOfCombatActionPattern = new FindAndFleeEntity(this, chasingAggroShape);
            OutOfCombatActionPattern.Loop = true;

            Animation = new Animation(new SpriteSheet_LizardRun());
        }
    }
}
