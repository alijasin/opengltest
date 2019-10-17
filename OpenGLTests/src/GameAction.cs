using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public interface IPlaceable
    {
        void Clicked();
        bool Placed(GameCoordinate location);
    }
    
    public enum ActionReturns
    {
        Finished,
        AllFinished,
        Ongoing,
        Placing,
        Invoked,
        NotReady
    }

    //todo split GameAction into Placed Action and Game Action
    public abstract class GameAction
    {
        public RangeShape RangeShape { get; set; }
        public abstract Func<object, bool> GetAction(); 
        public ActionMarker Marker { get; set; }
        public ActionLine ActionLine { get; set; }
        public bool Ready { get; set; } = true;
        public bool IsPlaced { get; set; } = false;



        //base GameAction properties
        protected GameAction()
        {
            RangeShape = new RangeCircle(new GLCoordinate(0, 0));
            ActionLine = new ActionLine(new GameCoordinate(0,0));
            Marker = new ActionMarker(new GameCoordinate(0,0));


            RangeShape.Visible = false;
            ActionLine.Visible = false;
            Marker.Visible = false;

            ActionLine.LineType = LineType.Solid;
        }

        public void SetMarkerIcon(SpriteID sid)
        {
            this.Marker.Animation.SetSprite(sid);
        }

        public void PositionLine(GameCoordinate origin, GameCoordinate terminus)
        {
            if (origin != null) ActionLine.Origin = origin;
            if (terminus != null) ActionLine.Terminus = terminus;
        }

        public void Dispose()
        {
            IsPlaced = false;

            Marker.Visible = false;
            ActionLine.Visible = false;
        }

        public void Place()
        {
            IsPlaced = true;

            Marker.Visible = true;
            ActionLine.Visible = true;
        }



        /*
        public abstract bool PayPreConditions();
        */
    }

    abstract class CombatAction : GameAction
    {

    }

    public abstract class ItemAction : GameAction
    {
        protected ItemAction()
        {
            ActionLine.LineType = LineType.Dashed;
        }
    }

    class TossItemAction : ItemAction
    {
        public TossItemAction(Entity source, Item i)
        {
            RangeShape = new Circle(new GLCoordinate(0.5f, 0.5f));
            RangeShape.Visible = false;
            Marker = new AOEMarker(new GameCoordinate(0.5f, 0.5f), new GLCoordinate(0.2f, 0.2f));
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Console.WriteLine("Tossed " + (int) o);
                if ((int) o >= 10) return true;
                return false;
            };
        }
    }

    class TurnRedAction : ItemAction
    {
        private Entity source;
        public TurnRedAction(Entity source)
        {
            this.source = source;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                source.Color = Color.Red;
                return true;
            };
        }
    }

    class GrowAction : ItemAction
    {
        private Entity source;
        public GrowAction(Entity source)
        {
            this.source = source;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                var index = (int) o;
                if(index < 10)
                {
                    source.Size.X = source.Size.X * 1.1f;
                    source.Size.Y = source.Size.Y * 1.1f;
                    return false;
                }

                return true;
            };
        }
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
            this.ActionLine.LineType = LineType.Solid;
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
            this.ActionLine.LineType = LineType.Dashed;
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
        private GLCoordinate initialAOERange;

        private AOEMarker marker => Marker as AOEMarker;

        public AOEEffectAction(GLCoordinate actionRange, GLCoordinate aoeRange)
        {
            RangeShape = new RangeCircle(actionRange);
            this.Marker = new AOEMarker(RangeShape.Location, aoeRange);
            
            initialAOERange = new GLCoordinate(aoeRange.X, aoeRange.Y);
            this.ActionLine.LineType = LineType.Dashed;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                int index = (int) o;
                if (marker.aoeSize.X < 0.0f || marker.aoeSize.Y < 0.0f || index > 100)
                {
                    marker.aoeSize = initialAOERange;
                    return true;
                }

                marker.aoeSize.X -= 0.01f;
                marker.aoeSize.Y -= 0.01f;

                return false;
            };
        }
    }

    class MoveTowardsEntityAction : GameAction
    {
        private Entity source;

        public MoveTowardsEntityAction(Entity source)
        {
            this.source = source;
            this.ActionLine.LineType = LineType.Solid;
        }
        
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Func<GameCoordinate> currentLocationMethod = (Func<GameCoordinate>) o;
                GameCoordinate point = currentLocationMethod.Invoke();

                //todo refactor this to outside helper function
                if (source.Location.Distance(point) < source.Speed.X || source.Location.Distance(point) < source.Speed.Y)
                {
                    //we are close enough
                    return true;
                }
                else
                {
                    var dx = point.X - source.Location.X;
                    var dy = point.Y - source.Location.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    var velX = (dx / dist) * source.Speed.X;
                    var velY = (dy / dist) * source.Speed.Y;

                    source.Location.X += (float)velX;
                    source.Location.Y += (float)velY;
                    return false;
                }
            };
        }
    }

    class MoveTowardsAction : GameAction
    {
        private Entity source;
        private GameCoordinate point;
        public MoveTowardsAction(GameCoordinate point, Entity source)
        {
            this.source = source;
            this.point = point;
            this.Marker = new MoveMarker(point);
            this.ActionLine.LineType = LineType.Solid;
            //GameState.Drawables.Add(this.Marker);
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (source.Location.Distance(point) < source.Speed.X || source.Location.Distance(point) < source.Speed.Y)
                {
                    //we are close enough
                    return true;
                }
                else
                {
                    var dx = point.X - source.Location.X;
                    var dy = point.Y - source.Location.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);
            
                    var velX = (dx / dist) * source.Speed.X;
                    var velY = (dy / dist) * source.Speed.Y;

                    source.Location.X += (float)velX;
                    source.Location.Y += (float) velY;
                    return false;
                }
            };
        }
    }

    class ChillAction : GameAction
    {
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (RNG.XTimesInY(2, 100)) return true;
                else return false;
            };
        }
    }

    class NeverEndingPatrolAction : GameAction
    {
        private MoveTowardsAction mtOrigin;
        private MoveTowardsAction mtTerminus;
        private MoveTowardsAction prevMoveTowardsAction;
        private MoveTowardsAction currentMoveTowards;
        //patrol between entity location and entity relative to point
        public NeverEndingPatrolAction(Entity e, GameCoordinate point)
        {
            mtOrigin = new MoveTowardsAction(e.Location + point, e);
            mtTerminus = new MoveTowardsAction(e.Location - new GameCoordinate(point.X * 2, point.Y * 2), e);
            currentMoveTowards = mtOrigin;
            prevMoveTowardsAction = mtTerminus;
        }


        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                bool res = currentMoveTowards.GetAction().Invoke(o);
                if (res == true)
                {
                    var temp = currentMoveTowards;
                    currentMoveTowards = prevMoveTowardsAction;
                    prevMoveTowardsAction = temp;
                }
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
            this.ActionLine.LineType = LineType.Solid;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                int index = (int)o;

                if (Marker != null)
                {
                    if (source.Location.Distance(Marker.Location) < source.Speed.X || source.Location.Distance(Marker.Location) < source.Speed.Y)
                    {
                        //we are close enough
                        return true;
                    }
                    var dx = Marker.Location.X - source.Location.X;
                    var dy = Marker.Location.Y - source.Location.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    var velX = (dx / dist) * source.Speed.X;
                    var velY = (dy / dist) * source.Speed.Y;

                    source.Location.X += (float)velX;
                    source.Location.Y += (float)velY;
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
