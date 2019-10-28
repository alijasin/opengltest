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
    //todo: dont chain interfaces. its jobbigt
    //todo: interface segragate
    public interface ICombatable : IFollowable
    {
        [JsonIgnore]
        RangeShape AggroShape { get; set; }
        bool InCombat { get; set; }
        void Step();
        GameCoordinate Location { get; set; }
        GameCoordinate Speed { get; set; }
        GLCoordinate Size { get; set; }
        Color Color { get; set; }
        int HitPoints { get; set; }
        void Damage(int dmg);
        void OnDeath();
        void OnAggro(ICombatable aggroed);
    }


    public abstract class Hostile : Entity, ICombatable
    {
        //[JsonConverter(typeof(RoomLoader.ConcreteConverter<RangeCircle>))]
        [JsonIgnore]
        public RangeShape AggroShape { get; set; } 
        protected ActionPattern ActionPattern;
        private ICombatable currentAggro;
        public bool InCombat { get; set; }
        public int HitPoints { get; set; }

        protected Hostile()
        {
            this.HitPoints = 1;
            Add();
        }

        public void Damage(int dmg)
        {
            HitPoints -= dmg;
            if(HitPoints <= 0) OnDeath();
        }

        public void OnDeath()
        {
            Console.WriteLine("{0} died.", this);
            if (currentAggro is Hero hero)
            {
                hero.Deaggro(this);
            }
            this.Dispose();
        }

        public void OnAggro(ICombatable aggroed)
        {
            currentAggro = aggroed;
            InCombat = true;
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
