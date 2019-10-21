using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    //todo: move stuff to Entity
    
    public interface ICombatable : IFollowable
    {
        RangeShape AggroShape { get; set; }
        bool InCombat { get; set; }
        void Step();
        GameCoordinate Location { get; set; }
        GameCoordinate Speed { get; set; }
        GLCoordinate Size { get; set; }
        Color Color { get; set; }
        int HitPoints { get; set; }
        void OnDeath();
    }


    abstract class Hostile : Entity, ICombatable
    {
        [JsonConverter(typeof(RoomLoader.ConcreteConverter<RangeCircle>))]
        public RangeShape AggroShape { get; set; } 
        protected ActionPattern ActionPattern;
        public bool InCombat { get; set; }
        public int HitPoints { get; set; }

        public void OnDeath()
        {
            Console.WriteLine("{0} died.", this);
            this.Dispose();
        }

        public void Step()
        {
            if(InCombat == false) OutOfCombatStep();  
        }

        public void EnteredCombat(ICombatable triggeringEntity)
        {
            //ActionPattern = new ChaseEntity(this, triggeringEntity);
        }

        public virtual void OutOfCombatStep()
        {
            if (ActionPattern != null)
            {
                var status = ActionPattern.DoAction(1);
            }
        }
        public Hostile()
        {
            this.HitPoints = 1;
        }

        public override void Dispose()
        {
            base.Dispose();
            AggroShape.Dispose();
            ActionPattern.Dispose();
        }
    }

    class AngryDude : Hostile
    {
        public AngryDude(GameCoordinate Location)
        {
            this.Location = Location;
            this.AggroShape = new RangeCircle(new GLCoordinate(0.3f, 0.3f), this);
            this.AggroShape.Visible = true;
            this.Speed = new GameCoordinate(0.01f, 0.01f);
            ActionPattern = new MoveAroundAndChill(this);
            ActionPattern.Loop = true;
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            AggroShape.DrawStep(drawer);
        }
    }

    class PatrolGuy : Hostile
    {
        public PatrolGuy(GameCoordinate location)
        {
            this.Speed = new GameCoordinate(0.01f, 0.005f);
            this.Location = location;

            ActionPattern = new NeverEndingPatrol(this, new GameCoordinate(0.2f, 0));
            ActionPattern.Loop = true;
        }
    }

    class ChasingPerson : Hostile
    {
        public ChasingPerson(GameCoordinate location, ICombatable chasing)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);

            ActionPattern = new ChaseEntity(this, chasing);
            ActionPattern.Loop = true;

        }
    }
}
