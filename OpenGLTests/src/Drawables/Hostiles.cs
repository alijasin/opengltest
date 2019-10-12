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
            pattern.Loop = true;
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
            AggroShape.Draw(drawer);
        }

        public override void Step()
        {
            base.Step();

            var status = pattern.DoAction("SkertSkert");
        }
    }

    class PatrolGuy : Hostile
    {
        private ActionPattern pattern;

        public PatrolGuy(GameCoordinate location)
        {
            this.Speed = new GameCoordinate(0.01f, 0.005f);
            this.Location = location;
            
            pattern = new NeverEndingPatrol(this, new GameCoordinate(0.2f, 0));// new PatrolAndChill(this, new GameCoordinate(0.5f, 0));
            pattern.Loop = true;
        }

        public override void Step()
        {
            base.Step();

            var status = pattern.DoAction("SkertSkert");
        }
    }

    class ChasingPerson : Hostile
    {
        private ActionPattern pattern;
        public ChasingPerson(GameCoordinate location, Entity chasing)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);
            pattern = new ChaseEntity(this, chasing);
            pattern.Loop = true;
        }

        public override void Step()
        {
            base.Step();

            var status = pattern.DoAction("ogelibogeli");
        }
    }
}
