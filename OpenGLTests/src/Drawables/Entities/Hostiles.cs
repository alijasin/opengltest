using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public abstract class Hostile : Unit, ICollidable
    {
        protected ActionPattern ActionPattern;
        private Unit currentAggro;
        public RangeShape BoundingBox { get; set; }

        protected Hostile()
        {
            this.HitPoints = 1;
            this.BoundingBox = new RangeShape(new Rectangle(new GLCoordinate(0.1f, 0.1f)), this);
            Add();
        }


        public override void OnDeath()
        {
            Console.WriteLine("{0} died.", this);
            if (currentAggro is Hero hero)
            {
                hero.Deaggro(this);
            }
            this.Dispose();
        }

        public override void OnAggro(Unit aggroed)
        {
            currentAggro = aggroed;
            InCombat = true;
        }

        public override void Step()
        {
            if(InCombat == false) OutOfCombatStep();  
        }

        public void EnteredCombat(Unit triggeringUnit)
        {
            //ActionPattern = new FindAndChaseEntity(this, triggeringEntity);
        }

        public virtual void OutOfCombatStep()
        {
            if (ActionPattern != null)
            {
                var status = ActionPattern.DoAction(1);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if(AggroShape != null) AggroShape.Dispose();
            if(ActionPattern != null) ActionPattern.Dispose();
        }

        protected void Add()
        {
            GameState.Drawables.Add(this);
            if(AggroShape != null) GameState.Drawables.Add(this.AggroShape);
        }

    }

}
