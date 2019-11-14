using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Util;
using OpenTK.Graphics.OpenGL4;

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
        public virtual bool IsPlaced { get; set; } = false;
        public bool IsInstant { get; set; } = false;
        public bool ForcePlaced { get; set; } = false;
        public virtual Func<GameCoordinate, bool> PlacementFilter { get; set; }
        public Unit Source { get; set; }
        protected int ActionPointCost = 0;
        public virtual Func<NPCState, GameCoordinate> NPCActionPlacementCalculator { get; set; } = (state) => new GameCoordinate(0, 0); 
        public GameCoordinate PlacedLocation { get; set; }
        public Animation IconAnimation { get; set; }
        public SpriteID Icon { get; set; }

        public GameAction() : this(null)
        {

        }
        protected GameAction(Unit source)
        {
            Source = source;

            this.IconAnimation = new Animation(new SpriteSheet_Icons());
            this.IconAnimation.SetSprite(SpriteID.missing);
            Marker = new ActionMarker(new GameCoordinate(0,0));
            Marker.Visible = false;
            PlacementFilter = DefaultPlacementFilter;
            if (source != null)
            {
                ActionLine = new ActionLine(source);
                ActionLine.Visible = false;
                ActionLine.LineType = LineType.Solid;

                RangeShape = new RangeShape(new Circle(new GLCoordinate(0f, 0f)), Marker);
            }
        }

        public bool DefaultPlacementFilter(GameCoordinate loc)
        {
            if (Source.AvailableActionPoints < this.ActionPointCost) return false;
            if (RangeShape == null) return true;
            if (RangeShape.Contains(loc)) return true;
            else
            {
                return RangeShape.IsInfinite || IsInstant;
            }
            return false;
        }

        public void Dispose()
        {
            IsPlaced = false;

            if(Marker != null) Marker.Visible = false;
            if(ActionLine != null) ActionLine.Visible = false;
            if(RangeShape != null) RangeShape.Visible = false;
        }

        public void Place(GameCoordinate location, SpriteID sid)
        {
            IsPlaced = true;

            this.PlacedLocation = location;
            this.Marker.Location = location;
            this.Marker.Animation.SetSprite(sid);
            this.ActionLine.Terminus = location;
            Marker.Visible = true;
            ActionLine.Visible = true;
        }

        public virtual void PayPreConditions()
        { 
            Source.AvailableActionPoints -= this.ActionPointCost;
        }

        public virtual Action OnSelected { get; set; } = () => { };
    }

    abstract class CombatAction : GameAction
    {
        protected CombatAction(Unit source) : base(source)
        {

        }
    }

    abstract class WeaponAction : CombatAction
    {
        protected WeaponAction(Unit source) : base(source)
        {

        }
    }

    

    class TurnAction : GameAction
    {
        private Facing facing;
        public TurnAction(Unit source, Facing facing) : base(source)
        {
            this.facing = facing;
        }

        public override Func<object, bool> GetAction()
        {
            return o =>
            {
                Source.SetFacing(facing);
                return true;
            };
        }
    }

    class IdleAction : GameAction
    {
        private int idleTime;

        public IdleAction(Unit source, int steps) : base(source)
        {
            this.idleTime = steps;
        }

        public override Func<object, bool> GetAction()
        {
            return o =>
            {
                int i = (int) o;

                if (i >= idleTime)
                {
                    return true;
                }

                return false;
            };
        }
    }

    class FindNearestUnit : GameAction
    {
        private Func<Unit, bool> targetFilter;
        public FindNearestUnit(Unit source, RangeShape withinRangeShape, Func<Unit, bool> targetFilter) : base(source)
        {
            this.targetFilter = targetFilter;
        }

        public override Func<object, bool> GetAction()
        {
            Console.WriteLine("FindNearestUnit game action was used but it is not implemented yet.");
            return o => true;
        }
    }

    class FireballAction : CombatAction
    {
        private Fireball fb;
        private Fan fan;
        private int degs;
        private float range = 0.7f;
        private int collisionDamage = 1;
        public FireballAction(Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.5f, 0.5f)), source);
            RangeShape.IsInfinite = true;
            degs = 3;
            fan = new Fan(range, degs);
            this.Marker = new AOEMarker(source.Location, new RangeShape(fan, source));
            this.ActionLine.LineType = LineType.Dashed;
            ActionPointCost = 1;

            this.NPCActionPlacementCalculator = state =>
            {
                if (Source is Hostile h)
                {
                    if (h.CurrentAggroTarget != null) return h.CurrentAggroTarget.Location;
                }
                return RNG.RandomPointWithinCircleRelativeToLocation(Source.Location, new GLCoordinate(1f, 1f));
            };
        }

        private GameCoordinate originLoc;
        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                int i = (int) o;
                if (i == 0)
                {
                    originLoc = new GameCoordinate(Source.Location.X, Source.Location.Y);
                    var dirAngle = MyMath.AngleBetweenTwoPoints(originLoc, PlacedLocation);
                    fb = new Fireball(originLoc, dirAngle, collisionDamage, Source);
                    GameState.Drawables.Add(fb);
                }

                var traveledDistance = MyMath.DistanceBetweenTwoPoints(fb.Location, originLoc);
   
                if (traveledDistance > range || fb.Finished)
                {
                    fb.Dispose();
                    return true;
                }

                return false;
            };
        }
    }


    class InstantTeleport : CombatAction
    {
        public InstantTeleport(GameCoordinate location, Unit source) : base(source)
        {
            PlacedLocation = location;
            NPCActionPlacementCalculator = state =>
            {
                return Source.Location + RNG.RandomPointWithinCircle(new GLCoordinate(0.2f, 0.2f));
            };
        }


        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                Source.Location = PlacedLocation;
                return true;
            };
        }
    }

    class HookShotAction : GameAction
    {
        private Unit theCollided;
        public HookShotAction(Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.8f, 0.7f)), source);
            this.Marker = new ActionMarker(RangeShape.Location);
            this.ActionLine.LineType = LineType.Solid;
            PlacementFilter += coordinate =>
            {
                return GameState.Drawables.GetAllCollidables.Any(d => d.BoundingBox.Contains(coordinate));
            };
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                return GameActionLambdas.MoveTowardsPoint(Source, Marker.Location, new GameCoordinate(0.05f, 0.05f));
            };
        }
    }

    class HookAction : GameAction
    {
        private Unit theCollided;
        public HookAction(Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.8f, 0.7f)), source);
            this.Marker = new ActionMarker(RangeShape.Location);
            this.ActionLine.LineType = LineType.Solid;
            this.ActionPointCost = 2;
            PlacementFilter = coordinate =>
            {
                //only placeable on collidables.
                bool collided = false;
                foreach (var d in GameState.Drawables.GetAllCollidables)
                {
                    if (d is Unit)
                    {
                        if (d.BoundingBox.Contains(coordinate))
                        {
                            collided = true;
                            theCollided = d as Unit;
                        }
                    }
                }
                return collided && DefaultPlacementFilter(coordinate);
                //return GameState.Drawables.GetAllCollidables.Any(d => d.BoundingBox.Contains(coordinate));
            };
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                return GameActionLambdas.MoveTowardsPoint(theCollided, Source.Location, new GameCoordinate(0.05f, 0.05f));
            };
        }
    }
    //todo: dont teleport within radius, but instead use a rangeshape
    class TeleportAction : CombatAction
    {
        private bool isOnCooldown = false;

        public TeleportAction(GLCoordinate radius, Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(radius), source);
            this.Marker = new MoveMarker(RangeShape.Location);
            this.ActionLine.LineType = LineType.Solid;
            //dont allow teleportation within collidables.
            this.PlacementFilter = coordinate => !GameState.Drawables.GetAllCollidables.Any(c => !c.Phased && c.BoundingBox.Contains(coordinate)) && DefaultPlacementFilter(coordinate);
            NPCActionPlacementCalculator = (state) =>
            {
                var h = Source as Hostile;
                var angle = MyMath.AngleBetweenTwoPoints(Source.Location, h.CurrentAggroTarget.Location);
                Console.WriteLine("Current " + h.Location);
                Console.WriteLine("new " + new GameCoordinate(h.Location.X + 0.2f * (float)Math.Cos(MyMath.DegToRad(angle)), h.Location.Y + 0.2f * (float)Math.Sin(MyMath.DegToRad(angle))));

                return new GameCoordinate(h.Location.X + radius.X*(float)Math.Cos(MyMath.DegToRad(angle)), h.Location.Y - radius.Y*(float)Math.Sin(MyMath.DegToRad(angle)));
            };
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
                    Source.Location = PlacedLocation;
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


    class SliceAction : WeaponAction
    {
        private Fan fan;
        private int degs;

        public SliceAction(Unit source) : base(source)
        {
            RangeShape = new RangeShape(new Circle(new GLCoordinate(0.5f, 0.5f)), source);
            RangeShape.IsInfinite = true;
            degs = 110;
            fan = new Fan(0.2f, degs);
            this.Marker = new AOEMarker(source.Location, new RangeShape(fan, source));
            this.ActionLine.LineType = LineType.Dashed;


            ActionPointCost = 1;
        }

        private int initAngle;
        public override Func<object, bool> GetAction()
        {
            return o =>
            {
                int i = (int) o;
                var angle = MyMath.AngleBetweenTwoPoints(Source.Location, Marker.Location);
                if (i == 0)
                {
                    initAngle = Source.Weapon.Rotation;
                    Source.Weapon.Color = Color.GreenYellow;
                    Source.Weapon.Size.Y *= 2;
                }
                else
                {
                    Source.Weapon.Rotation = angle - 90 + degs/2 - i*10;
                }

                if (i*10 > degs)
                {
                    Source.Weapon.Rotation = initAngle;
                    Source.Weapon.Color = Color.White;
                    Source.Weapon.Size = new GLCoordinate(Source.Weapon.InitialSize.X, Source.Weapon.InitialSize.Y);
                    foreach (var u in GameState.Drawables.GetAllUnits)
                    {
                        if (u != Source)
                        {
                            if (fan.Contains(u.Location , Source.Location, angle))
                            {
                                u.Damage(1);
                            }
                        }
                        
                    }
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

    class SpawnBearTrapAction : GameAction
    {
        private BearTrap bt;
        private bool onlyOne;
        public SpawnBearTrapAction(Unit source, bool onlyOne = true) : base(source)
        {
            this.RangeShape = new RangeShape(new Circle(new GLCoordinate(0.4f, 0.4f)), source);
            this.Marker = new ActionMarker(RangeShape.Location);
            this.onlyOne = onlyOne;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (bt == null || onlyOne == false)
                {
                    bt = new BearTrap(PlacedLocation);
                    bt.Location = Marker.Location;
                    GameState.Drawables.Add(bt);
                }
                else
                {
                    bt.Location = Marker.Location;
                    bt.Reset();
                }
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
        public ChillAction(int n = 1, int r = 100)
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

    //this really shouldnt be unlike hero move action. Todo
    class UnitMoveAction : GameAction
    {
        public UnitMoveAction(Unit source, GameCoordinate moveTo) : base(source)
        {
            PlacedLocation = moveTo;
            source.MovingTowardsPoint = PlacedLocation;
            this.NPCActionPlacementCalculator = state =>
            {
                return RNG.RandomPointWithinCircleRelativeToLocation(Source.Location, new GLCoordinate(0.2f, 0.2f));
            };
        }

        public override Func<object, bool> GetAction()
        {
            return o =>
            {
                int index = (int)o;
                if (index > 150) return true; //dont get stuck
                Source.MovingTowardsPoint = PlacedLocation;
                return GameActionLambdas.MoveTowardsPoint(Source, PlacedLocation); ;
            };
        }
    }

    class HeroMoveAction : GameAction
    {
        private bool isOnCooldown = false;

        public HeroMoveAction(GLCoordinate radius, Unit hero) : base(hero)
        {
            RangeShape = new RangeShape(new Circle(radius), hero);
            this.Marker = new MoveMarker(RangeShape.Location);
            this.ActionLine.LineType = LineType.Solid;
            this.ActionPointCost = 1;
        }

        public override Func<object, bool> GetAction()
        {
            return (o) =>
            {
                if (Source.InCombat) return combatAction(o);
                else return outOfCombatAction(o);
            };
        }

        private bool outOfCombatAction(object arg)
        {
            int index = (int)arg;
            if (index > 200) return true; //dont get stuck
            if (Marker != null)
            {
                return GameActionLambdas.MoveTowardsPoint(Source, PlacedLocation);
            }
            return false;
        }

        private bool combatAction(object arg)
        {
            int index = (int)arg;
            if (index > 200) return true; //dont get stuck
            if (Marker != null)
            {
                return GameActionLambdas.MoveTowardsPoint(Source, Marker.Location, typeof(Unit));
            }
            return false;
        }
    }

}
