using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public abstract class GameAction
    {
        public RangeShape RangeShape { get; set; }
        //public float Range { get; set; } = 0;
        public abstract Action GetAction();
        /*public abstract void PostUpdateAction(Entity source, Entities others, GameCoordinate clicked);
        public abstract bool PayPreConditions();*/
    }

    abstract class CombatAction : GameAction
    {

    }

    class ChargeAction : CombatAction
    {
        private bool isOnCooldown = false;
        private Entity source;

        public ChargeAction(GLCoordinate radius, Entity source)
        {
            RangeShape = new RangeCircle(radius);
            this.source = source;
        }

        public override Action GetAction()
        {
            return () =>
            {
                if (RangeShape.Marker != null)
                {
                    source.Location = RangeShape.Marker.Location;
                }
            };
        }

    }

    class LambdaAction : GameAction
    {
        private System.Action a; 
        public LambdaAction(System.Action action)
        {
            RangeShape = new RangeCircle(new GLCoordinate(0.8f, 0.8f));
            a = action;
        }

        public override Action GetAction()
        {
            return a.Invoke;
        }
    }

    class MoveAction : CombatAction
    {
        private bool isOnCooldown = false;
        private Entity source;

        public MoveAction(GLCoordinate radius, Entity source)
        {
            RangeShape = new RangeCircle(radius);
            this.source = source;
        }

        public override Action GetAction()
        {
            return () =>
            {
                if (RangeShape.Marker != null)
                {
                    source.Location = RangeShape.Marker.Location;
                }
            };
        }

        /*public override void PostUpdateAction(Entity source, Entities others, GameCoordinate clicked)
        {
            // this.Marker.Location = clicked;
        }*/

        /*public override bool PayPreConditions()
        {
            if (isOnCooldown == false)
            {
                isOnCooldown = true;
                return true;
            }

            return false;
        }*/

    }
}
