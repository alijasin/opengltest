﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    //todo: move stuff to Entity
    public interface ICombatable : IFollowable
    {
        bool InCombat { get; }
        void Step();
        GameCoordinate Location { get; set; }
        GameCoordinate Speed { get; set; }
        GLCoordinate Size { get; set; }
        Color Color { get; set; }
    }

    interface IAggro : ICombatable
    {
        RangeShape AggroShape { get; set; }
    }

    abstract class Hostile : Entity, IAggro, ICombatable
    {
        public RangeShape AggroShape { get; set; }
        protected ActionPattern ActionPattern;
        public bool InCombat { get; set; }
        public void Step()
        {
            if(InCombat == false) OutOfCombatStep();  
        }

        private void OutOfCombatStep()
        {
            if (ActionPattern != null)
            {
                var status = ActionPattern.DoAction("SkertSkert");
            }

            //check for combat
            if (AggroShape == null) return;

            foreach (Hero h in GameState.Drawables.GetAllHeroes)
            {
                AggroShape.Location = Location; //might not need..
                if (AggroShape.Contains(h.Location))
                {
                    InCombat = true;
                    h.SetCombat(true);
                    ActionPattern = new ChaseEntity(this, h);
                    GameState.Combat = true;
                }
            }
        }

        protected Hostile()
        {
            AggroShape = new RangeCircle(new GLCoordinate(0, 0), this);
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
