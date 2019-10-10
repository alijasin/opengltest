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
        public abstract Func<object, bool> GetAction();
        public Marker Marker { get; set; }
        /*
        public abstract bool PayPreConditions();
        */
    }

    abstract class CombatAction : GameAction
    {

    }

    class TeleportAction : CombatAction
    {
        private bool isOnCooldown = false;
        private Entity source;

        public TeleportAction(GLCoordinate radius, Entity source)
        {
            RangeShape = new RangeCircle(radius);
            this.Marker = new MoveMarker(RangeShape.Location);
            this.source = source;
        }

        public override Func<object, bool> GetAction()
        {
            return (a) =>
            {
                if (Marker != null)
                {
                    source.Location = Marker.Location;
                }
                return true;
            };
        }

    }

    class LambdaAction : GameAction
    {
        private Func<object, bool> a; 
        public LambdaAction(Func<object, bool> f)
        {
            RangeShape = new RangeCircle(new GLCoordinate(0.8f, 0.8f));
            this.Marker = new ActionMarker(RangeShape.Location);
            a = f;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                a.Invoke(o);
                return true;
            };
        }
    }

    class MoveAction : CombatAction
    {
        private bool isOnCooldown = false;
        private Entity source;

        public MoveAction(GLCoordinate radius, Entity source)
        {
            RangeShape = new RangeCircle(radius);
            this.Marker = new MoveMarker(RangeShape.Location);
            this.source = source;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                int index = (int)o;

                if (Marker != null)
                {
                    if (source.Location.Distance(Marker.Location) < source.Speed.X) return true;

                    if (source.Location.X < Marker.Location.X)
                    {
                        source.Location.X += source.Speed.X;
                    }
                    else
                    {
                        source.Location.X -= source.Speed.X;
                    }

                    if (source.Location.Y < Marker.Location.Y)
                    {
                        source.Location.Y += source.Speed.Y;
                    }
                    else
                    {
                        source.Location.Y -= source.Speed.Y;
                    }
                }

                if (index < 100) return false; //dont get stuck
                return true;
            };
        }

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
