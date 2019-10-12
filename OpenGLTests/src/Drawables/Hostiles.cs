using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    interface IAggro
    {
        RangeShape AggroShape { get; set; }
    }

    abstract class Hostile : Entity, IAggro
    {
        public RangeShape AggroShape { get; set; } = new Circle(new GLCoordinate(0.2f, 0.2f));
    }

    class AngryDude : Hostile
    {
        private ActionPattern pattern;
        public AngryDude()
        {
            this.AggroShape = new FollowCircle(new GLCoordinate(0.3f, 0.3f), this);
            this.AggroShape.Visible = true;
            this.Speed = new GameCoordinate(0.01f, 0.01f);
            pattern = new MoveAroundAndChill(this);
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
            AggroShape.Draw(drawer);
        }

        public override void Step()
        {
            base.Step();

            if (pattern.Actions.First().GetAction().Invoke("skrt"))
            {
                pattern.Actions.Remove(pattern.Actions.First());
            }

            if (pattern.Actions.Count == 0)
            {
                pattern = new MoveAroundAndChill(this);
            }
            /*if (CurrentlyDoing != null)
            {
                var donedidely = CurrentlyDoing.GetAction().Invoke("skobidobido");
                if (donedidely)
                {
                    CurrentlyDoing = new MoveTowardsAction(RNG.RandomPointWithinCircle(new GLCoordinate(0.5f, 0.5f)), this);
                }
            }
            else
            {
                CurrentlyDoing = new MoveTowardsAction(new GameCoordinate(-0.4f, 0.4f), this);
            }*/
        }
    }
}
