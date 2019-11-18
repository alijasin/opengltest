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
    public enum NPCState
    {
        Scared,
        Angry,

    }
    public abstract class Hostile : Unit, ICollidable
    {
        protected ActionPattern OutOfCombatActionPattern;
        protected ActionPattern CombatActionPattern;
        public Unit CurrentAggroTarget;
        protected Hostile()
        {
            this.HitPoints = 1;
            this.BoundingBox = new RangeShape(new Rectangle(new GLCoordinate(0.1f, 0.1f)), this);
            this.ActionHandler = new OutOfCombatActionHandler(this);
            Add();
        }

        public override int FacingAngle
        {
            get { return MyMath.AngleBetweenTwoPoints(this.Location, MovingTowardsPoint); }
        }

        public override void OnDeath()
        {
            if (HitPoints < 0)
            {
                Console.WriteLine(this.GetType() + " died but is already dead. hmm..");
                return;
            }
            base.OnDeath();
            Console.WriteLine("{0} died.", this);
            if (CurrentAggroTarget is Hero hero)
            {
                hero.Deaggro(this);
            }
            this.Dispose();
        }

        public override void OnAggro(Unit aggroed)
        {
            if (InCombat) return;
            ActionHandler.Dispose();
            ActionHandler = new NPCCombatActionHandler(this);
            CurrentAggroTarget = aggroed;
            if(InCombat == false) EnteredCombat(aggroed);
            InCombat = true;
            
        }

        public override void CombatStep(Fight fight)
        {
            //var status = ActionHandler.CommitActions(CombatIndex);
            var status = CombatActionPattern.DoAction(CombatIndex);
            if (status == ActionReturns.Placing) return;
            if (status == ActionReturns.Finished)
            {
                CombatIndex = 0;
                return;
            }
            else if (status == ActionReturns.AllFinished || status == ActionReturns.NoAction)
            {
                CombatIndex = 0;
                EndedTurn = false;
                fight.UnitFinishedTurn(this);
            }
            else CombatIndex++;
        }

        public override void OutOfCombatStep()
        {
            if (OutOfCombatActionPattern != null)
            {
                var status = OutOfCombatActionPattern.DoAction(OutOfCombatIndex);
                if (status == ActionReturns.AllFinished || status == ActionReturns.Finished) OutOfCombatIndex = 0;
                else OutOfCombatIndex++;
            }
        }

        public void EnteredCombat(Unit triggeringUnit)
        {

        }

        public override void OnPreTurn()
        {
            if (CombatActionPattern == null || ActionHandler == null) return;

            foreach (var ca in CombatActionPattern.Actions)
            {
                var combatActionHandler = ActionHandler as NPCCombatActionHandler; //todo dont.
                combatActionHandler.TryPlaceAction(ca, NPCState.Angry);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if(AggroShape != null) AggroShape.Dispose();
            if(OutOfCombatActionPattern != null) OutOfCombatActionPattern.Dispose();
        }

        protected void Add()
        {
            GameState.Drawables.Add(this);
            if(AggroShape != null) GameState.Drawables.Add(this.AggroShape);
        }
    }
}
