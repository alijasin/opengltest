using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public interface ICombatable
    {
        bool InCombat { get; set; }

        void CombatStep();

        void OutOfCombatStep();

    }

    interface IAggro : ICombatable
    {
        RangeShape AggroShape { get; set; }
    }

    abstract class Hostile : Entity, IAggro
    {
        public RangeShape AggroShape { get; set; }
        protected ActionPattern ActionPattern;
        public bool InCombat { get; set; }

        public void CombatStep()
        {
            
        }

        public void OutOfCombatStep()
        {
            if (ActionPattern != null)
            {
                var status = ActionPattern.DoAction("SkertSkert");
            }

            //check for combat
            if (AggroShape == null) return;
            foreach(Hero h in GameState.Drawables.GetAllHeroes)
            {
                if (AggroShape.Contains(h.Location))
                {
                    InCombat = true;
                    h.InCombat = true;
                    ActionPattern = new ChaseEntity(this, h);
                    GameState.Combat = true;
                }
            }

        }
    }

    class AngryDude : Hostile
    {
        public AngryDude()
        {
            this.AggroShape = new FollowCircle(new GLCoordinate(0.3f, 0.3f), this);
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
        public ChasingPerson(GameCoordinate location, Entity chasing)
        {
            this.Location = location;
            this.Speed = new GameCoordinate(0.001f, 0.001f);
            ActionPattern = new ChaseEntity(this, chasing);
            ActionPattern.Loop = true;
        }
    }
}
