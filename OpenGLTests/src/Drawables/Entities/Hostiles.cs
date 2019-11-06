﻿using System;
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
            ActionHandler = new NPCCombatActionHandler(this);
            currentAggro = aggroed;
            if(InCombat == false) EnteredCombat(aggroed);
            InCombat = true;
            
        }

        public override void CombatStep(Fight fight)
        {
            var status = ActionHandler.CommitActions(CombatIndex);

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
