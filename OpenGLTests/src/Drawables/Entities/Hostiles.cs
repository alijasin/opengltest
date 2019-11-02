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
    public abstract class Hostile : Unit, ICollidable, IActionCapable
    {
        protected ActionPattern ActionPattern;
        private Unit currentAggro;
        public bool Phased { get; set; } = false;
        public RangeShape BoundingBox { get; set; }
        protected Hostile()
        {
            this.HitPoints = 1;
            this.BoundingBox = new RangeShape(new Rectangle(new GLCoordinate(0.1f, 0.1f)), this);
            this.ActionHandler = new OutOfCombatActionHandler(this);
            Add();
        }


        public override void OnDeath()
        {
            base.OnDeath();
            Console.WriteLine("{0} died.", this);
            if (currentAggro is Hero hero)
            {
                hero.Deaggro(this);
            }
            this.Dispose();
        }

        public override void OnAggro(Unit aggroed)
        {
            if (InCombat) return;
            ActionHandler.Dispose();
            ActionHandler = new CombatActionHandler(this);
            currentAggro = aggroed;
            if(InCombat == false) EnteredCombat(aggroed);
            InCombat = true;
            
        }


        public override void OutOfCombatStep()
        {
            if (ActionPattern != null)
            {
                var status = ActionPattern.DoAction(OutOfCombatIndex);
                if (status == ActionReturns.AllFinished || status == ActionReturns.Finished) OutOfCombatIndex = 0;
                else OutOfCombatIndex++;
            }
        }

        public void EnteredCombat(Unit triggeringUnit)
        {

        }

        public override void OnPreTurn()
        {
            ActionHandler.TryPlaceAction(new GrowAction(this), this.Location);
            ActionHandler.TryPlaceAction(new TurnRedAction(this), this.Location);
            var xd = new InstantTeleport(this.Location + new GameCoordinate(0.4f, 0.4f), this);
            xd.ForcePlaced = true;
            ActionHandler.TryPlaceAction(xd, this.Location + new GameCoordinate(0.4f, 0.4f));
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
