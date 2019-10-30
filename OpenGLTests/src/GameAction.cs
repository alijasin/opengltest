﻿using System;
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
        NotReady,
        NoAction
    }

    //todo split GameAction into Placed Action and Game Action and target action and so on.
    public abstract class GameAction
    {
        public RangeShape RangeShape { get; set; }
        public abstract Func<object, bool> GetAction(); 
        public ActionMarker Marker { get; set; }
        public ActionLine ActionLine { get; set; }
        public bool Ready { get; set; } = true;
        public bool IsPlaced { get; set; } = false;
        public bool IsInstant { get; set; } = false;
        protected Unit Source { get; set; }
        public GameAction() : this(null)
        {

        }
        protected GameAction(Unit source)
        {
            Source = source;

            Marker = new ActionMarker(new GameCoordinate(0,0));
            Marker.Visible = false;
            if (source != null)
            {
                ActionLine = new ActionLine(source);
                ActionLine.Visible = false;
                ActionLine.LineType = LineType.Solid;

                RangeShape = new RangeShape(new Circle(new GLCoordinate(0f, 0f)), Marker);
            }
        }

        public void SetMarkerIcon(SpriteID sid)
        {
            this.Marker.Animation.SetSprite(sid);
        }

        public void Dispose()
        {
            IsPlaced = false;

            Marker.Visible = false;
            ActionLine.Visible = false;
        }

        public void Place(GameCoordinate location, SpriteID sid)
        {
            IsPlaced = true;

            this.Marker.Location = location;
            this.Marker.Animation.SetSprite(sid);
            this.ActionLine.Terminus = location;
            Marker.Visible = true;
            ActionLine.Visible = true;
        }

        public void SetRangeShape(RangeShape r)
        {
            this.RangeShape = r;
        }
    }

    abstract class CombatAction : GameAction
    {
        protected CombatAction(Unit source) : base(source)
        {

        }
    }

    public abstract class ItemAction : GameAction
    {
        protected ItemAction(Unit source) : base(source)
        {
            ActionLine.LineType = LineType.Dashed;
        }
    }

    class TossItemAction : ItemAction
    {
        public TossItemAction(Unit source, Item i) : base(source)
        {
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.5f, 0.5f)), source);
            RangeShape.Visible = false;
            Marker = new AOEMarker(new GameCoordinate(0.5f, 0.5f), new RangeShape(new Circle(new GLCoordinate(0.05f, 0.05f)), source));
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if ((int) o >= 10)
                {
                    var aoeShape = (Marker as AOEMarker).aoeShape;
                    foreach (var others in GameState.Drawables.GetAllUnits.Where(d => d != Source))
                    {
                        if (aoeShape.Contains(others.Location))
                        {
                            others.Damage(1);
                        }
                    }

                    return true;
                }
                return false;
            };
        }
    }

    class TurnRedAction : ItemAction
    {
        public TurnRedAction(Unit source) : base(source)
        {
            IsInstant = true;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Source.Color = Color.Red;
                return true;
            };
        }
    }

    class GrowAction : ItemAction
    {
        public GrowAction(Unit source) : base(source)
        {
            IsInstant = true;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                var index = (int) o;
                Console.WriteLine(index);
                if (index < 10)
                {
                    Source.Size.X = Source.Size.X * 1.1f;
                    Source.Size.Y = Source.Size.Y * 1.1f;
                    return false;
                }
                return true;
            };
        }
    }

    class InstantTeleport : CombatAction
    {
        private GameCoordinate loc;
        public InstantTeleport(GameCoordinate location, Unit source) : base(source) 
        {
            this.loc = location;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Console.WriteLine(Source.Location);
                Console.WriteLine(loc);
                Source.Location = loc;
                return true;
            };
        }
    }

    //todo: dont teleport within radius, but instead of a rangeshape
    class TeleportAction : CombatAction
    {
        private bool isOnCooldown = false;

        public TeleportAction(GLCoordinate radius, Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(radius), source);
            this.Marker = new MoveMarker(RangeShape.Location);
            this.ActionLine.LineType = LineType.Solid;
        }

        public override Func<object, bool> GetAction()
        {
            return (a) =>
            {
                Color c = Source.Color;
                var index = (int)a;

                if (index < 15)
                {
                    Source.Color = Color.FromArgb((int)(c.A/(1.2)), c);
                }

                if (index == 15)
                { 
                    Source.Location = Marker.Location;
                }

                if (index > 15)
                {
                    Source.Color = Color.FromArgb((int)(c.A *1.2)%255, Source.Color);
                }

                if (index > 30)
                {
                    Source.Color = Color.FromArgb(255, Source.Color);
                    return true;
                }

                return false;
            };
        }

    }

    class LambdaAction : GameAction
    {
        private Func<object, bool> a; 
        public LambdaAction(Func<object, bool> f, Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.8f, 0.8f)), source);
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

        public AOEEffectAction(GLCoordinate actionRange, RangeShape aoeShape, Unit source) : base(source)
        {
            new RangeShape(new Circle(actionRange), source);
            this.Marker = new AOEMarker(RangeShape.Location, aoeShape);
            
            initialAOERange = new GLCoordinate(aoeShape.Size.X, aoeShape.Size.Y);
            this.ActionLine.LineType = LineType.Dashed;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {/*
                int index = (int) o;
                if (marker.aoeSize.X < 0.0f || marker.aoeSize.Y < 0.0f || index > 100)
                {
                    marker.aoeSize = initialAOERange;
                    return true;
                }

                marker.aoeSize.X -= 0.01f;
                marker.aoeSize.Y -= 0.01f;

                return false;*/
                return true;
            };
        }
    }

    class FindAndChase : GameAction
    {
        private RangeShape findRad;
        public FindAndChase(RangeShape findShape, Unit source) : base(source)
        {
            findRad = findShape;
        }

        private bool looking = true;
        private Unit chasing;
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (looking)
                {
                    chasing = GameActionLambdas.FindHeroLambda(findRad);
                    if (chasing != null) looking = false;
                }
                else
                {
                    Source.Color = Color.Red;
                    var res = GameActionLambdas.MoveTowardsPoint(Source, chasing.Location);
                    if (res) Source.Color = Color.White;
                    return res;
                }
                return false;
            };
        }
    }

    class FindAndFlee : GameAction
    {
        private RangeShape findRad;
        public FindAndFlee(RangeShape findShape, Unit source) : base(source)
        {
            findRad = findShape;
        }

        private bool looking = true;
        private Unit chasing;
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (looking)
                {
                    chasing = GameActionLambdas.FindHeroLambda(findRad);
                    if (chasing != null) looking = false;
                }
                else
                {
                    Source.Color = Color.Green;
                    var res = GameActionLambdas.MoveAwayFromPoint(Source, chasing.Location);
                    if (res) Source.Color = Color.White;
                    return res;
                }
                return false;
            };
        }
    }


    class MoveAwayFromEntityAction : GameAction
    {
        public MoveAwayFromEntityAction(Unit source) : base(source)
        {
            this.ActionLine.LineType = LineType.Solid;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Func<GameCoordinate> currentLocationMethod = (Func<GameCoordinate>)o;
                GameCoordinate point = currentLocationMethod.Invoke();

                //todo refactor this to outside helper function
                if (Source.Location.Distance(point) < Source.Speed.X || Source.Location.Distance(point) < Source.Speed.Y)
                {
                    //will never return true if unit is free to move. 
                    //should return true on outside of range.
                    //todo.
                    return true;
                }
                else
                {
                    var dx = point.X - Source.Location.X;
                    var dy = point.Y - Source.Location.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    var velX = (dx / dist) * Source.Speed.X;
                    var velY = (dy / dist) * Source.Speed.Y;

                    Source.Location.X -= (float)velX;
                    Source.Location.Y -= (float)velY;
                    return false;
                }
            };
        }
    }
    class MoveTowardsEntityAction : GameAction
    {
        public MoveTowardsEntityAction(Unit source) : base(source)
        {
            this.ActionLine.LineType = LineType.Solid;
        }
        
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Func<GameCoordinate> currentLocationMethod = (Func<GameCoordinate>) o;
                GameCoordinate point = currentLocationMethod.Invoke();

                //todo refactor this to outside helper function
                if (Source.Location.Distance(point) < Source.Speed.X || Source.Location.Distance(point) < Source.Speed.Y)
                {
                    //we are close enough
                    return true;
                }
                else
                {
                    var dx = point.X - Source.Location.X;
                    var dy = point.Y - Source.Location.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);

                    var velX = (dx / dist) * Source.Speed.X;
                    var velY = (dy / dist) * Source.Speed.Y;

                    Source.Location.X += (float)velX;
                    Source.Location.Y += (float)velY;
                    return false;
                }
            };
        }
    }


    class MoveAction : GameAction
    {
        private GameCoordinate point;

        public MoveAction(GameCoordinate point, Unit source) : base(source)
        {
            this.point = point;
            this.Marker = new MoveMarker(point);
            this.ActionLine.LineType = LineType.Solid;
            this.Marker.Visible = false;
            this.Marker.Animation = new Animation(new SpriteSheet_Icons());
            this.Marker.Animation.SetSprite(SpriteID.action_move);
            this.Marker.Size = new GLCoordinate(0.05f, 0.05f);
            this.RangeShape.IsInfinite = true;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (Source.Location.Distance(point) < Source.Speed.X || Source.Location.Distance(point) < Source.Speed.Y)
                {
                    //we are close enough
                    this.Marker.Dispose();
                    return true;
                }
                else
                {
                    var dx = point.X - Source.Location.X;
                    var dy = point.Y - Source.Location.Y;
                    var dist = Math.Sqrt(dx * dx + dy * dy);
            
                    var velX = (dx / dist) * Source.Speed.X;
                    var velY = (dy / dist) * Source.Speed.Y;

                    Source.Location.X += (float)velX;
                    Source.Location.Y += (float) velY;
                    return false;
                }
            };
        }
    }

    class ChillAction : GameAction
    {
        private int n;
        private int r;
        /// <summary>
        /// true n out of r times.
        /// </summary>
        /// <param name="n">times out of</param>
        /// <param name="r">r</param>
        public ChillAction(int n = 2, int r = 100)
        {
            this.n = n;
            this.r = r;
        }
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (RNG.XTimesInY(n, r)) return true;
                else return false;
            };
        }
    }

    class NeverEndingPatrolAction : GameAction
    {
        private MoveAction mtOrigin;
        private MoveAction mtTerminus;
        private MoveAction _prevMoveAction;
        private MoveAction _currentMove;
        //patrol between ICombatable location and ICombatable relative to point
        public NeverEndingPatrolAction(Unit e, GameCoordinate point) : base(e)
        {
            mtOrigin = new MoveAction(e.Location + point, e);
            mtTerminus = new MoveAction(e.Location - new GameCoordinate(point.X * 2, point.Y * 2), e);
            _currentMove = mtOrigin;
            _prevMoveAction = mtTerminus;
        }


        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                bool res = _currentMove.GetAction().Invoke(o);
                if (res == true)
                {
                    var temp = _currentMove;
                    _currentMove = _prevMoveAction;
                    _prevMoveAction = temp;
                }
                return false;
            };
        }
    }

    class HeroMoveAction : CombatAction
    {
        private bool isOnCooldown = false;

        public HeroMoveAction(GLCoordinate radius, Hero hero) : base(hero)
        {
            RangeShape = new RangeShape(new Circle(radius), hero);
            this.Marker = new MoveMarker(RangeShape.Location);
            this.ActionLine.LineType = LineType.Solid;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) => { return combatAction(o); };
        }

        private bool combatAction(object arg)
        {
            int index = (int)arg;
            if (index > 200) return true; //dont get stuck
            if (Marker != null)
            {
                if (Source.Location.Distance(Marker.Location) < Source.Speed.X || Source.Location.Distance(Marker.Location) < Source.Speed.Y)
                {
                    //we are close enough
                    return true;
                }

                //todo heromove action should take hero and not icombatable
                var h = Source as Hero;

                if (Source.Location.X < Marker.Location.X)
                {
                    h.Facing = Facing.Right;
                }
                else
                {
                    h.Facing = Facing.Left;
                }


                var dx = Marker.Location.X - Source.Location.X;
                var dy = Marker.Location.Y - Source.Location.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                var velX = (float)(dx / dist) * Source.Speed.X;
                var velY = (float)(dy / dist) * Source.Speed.Y;


                bool blockedX = false;
                bool blockedY = false;

                //:: Optimizable Area
                //1. filter these collidables some more so we dont iterate all of them.
                //2. store some things in variables
                foreach (var collidable in GameState.Drawables.GetAllCollidables)
                {
                    if (collidable.BoundingBox.Contains(new GameCoordinate(collidable.BoundingBox.Location.X,Source.Location.Y + velY*2))
                         && collidable.BoundingBox.Contains(new GameCoordinate(Source.Location.X + velX/2, collidable.BoundingBox.Location.Y)))
                    {
                        blockedY = true;
                    }
                    if (collidable.BoundingBox.Contains(new GameCoordinate(Source.Location.X + velX * 2, collidable.BoundingBox.Location.Y))
                        && collidable.BoundingBox.Contains(new GameCoordinate(collidable.BoundingBox.Location.X, Source.Location.Y + velY/2)))
                    {
                        blockedX = true;
                    }
                }

                if (!blockedY) Source.Location.Y += velY;
                if (!blockedX) Source.Location.X += velX;

            }


            return false;
        }
    }
}
