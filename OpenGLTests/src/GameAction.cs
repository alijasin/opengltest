using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public abstract class GameAction
    {
        public RangeShape RangeShape { get; set; }
        public abstract Func<object, bool> GetAction(); //todo, add post action stuff to all functions.
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
            Color initialColor = source.Color;

            return (a) =>
            {
                Color c = source.Color;
                var index = (int)a;
                if (index == 0)
                {
                    initialColor = c;
                }

                if (index < 15)
                {
                    source.Color = Color.FromArgb((int)(c.A/(1.2)), c);
                }
                if (index == 15)
                { 
                    source.Location = Marker.Location;
                }

                if (index > 15)
                {
                    source.Color = Color.FromArgb((int)(c.A *1.2)%255, source.Color);
                }

                if (index == 30)
                {
                    source.Color = Color.FromArgb(255, source.Color);
                    return true;
                }

                return false;
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

    class AOEEffectAction : CombatAction
    {
        private AOEMarker m;
        private GLCoordinate initialAOERange;
        public AOEEffectAction(GLCoordinate actionRange, GLCoordinate aoeRange)
        {
            RangeShape = new RangeCircle(actionRange);
            m = new AOEMarker(RangeShape.Location, aoeRange);
            initialAOERange = new GLCoordinate(aoeRange.X, aoeRange.Y);
            this.Marker = m;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                int index = (int) o;

                if (m.aoeSize.X < 0.0f || m.aoeSize.Y < 0.0f || index > 100)
                {
                    m.aoeSize = initialAOERange;
                    return true;
                }

                m.aoeSize.X -= 0.01f;
                m.aoeSize.Y -= 0.01f;

                return false;
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
