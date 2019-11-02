using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    /// <summary>
    /// Contains a list of game actions that will be executed and returns the execution status.
    /// </summary>
    public abstract class ActionPattern
    {
        public GameAction CurrentAction => Actions.First();
        public abstract void InitPattern();
        public List<GameAction> Actions;
        public bool Loop = false;

        public virtual ActionReturns DoAction(object arg)
        {
            if (Actions.Count == 0)
            {
                //reinitialize the pattern - all randomness will be regenerated.
                if(Loop) InitPattern();
                return ActionReturns.AllFinished;
            }
            var currentGameAction = Actions.First();
            var actionFinished = currentGameAction.GetAction().Invoke(arg);
            if (actionFinished)
            {
                Actions.Remove(currentGameAction);
                return ActionReturns.Finished;
            }
            else
            {
                return ActionReturns.Ongoing;
            }
        }

        public void Dispose()
        {
            foreach (var a in Actions)
            {
                if(a.Marker != null) a.Marker.Dispose();
                if(a.ActionLine != null) a.ActionLine.Dispose();
                GameState.Drawables.Remove(a.RangeShape);
            }
        }
    }

    class CustomPattern : ActionPattern
    {
        public CustomPattern(List<GameAction> actions)
        {
            Actions = actions;
        }
        public override void InitPattern()
        {
            
        }
    }

    class SwamperTeleportPattern : ActionPattern
    {
        private GLCoordinate range;
        private Unit source;
        public SwamperTeleportPattern(Unit source, GLCoordinate range)
        {
            this.source = source;
            this.range = range;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new InstantTeleport(RNG.RandomPointWithinCircleRelativeToLocation(source.Location, range), source),
            };
        }
    }

    class FindAndChaseEntity : ActionPattern
    {
        private Unit chasing;
        private Unit source;

        public FindAndChaseEntity(Unit source)
        {
            this.source = source;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new FindAndChase(new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), source), source),
            };
        }
    }
    class FindAndFleeEntity : ActionPattern
    {
        private Unit fleeing;
        private Unit source;

        public FindAndFleeEntity(Unit source)
        {
            this.source = source;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new FindAndFlee(new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), source), source),
            };
        }
    }
    class MoveAroundAndChill : ActionPattern
    {
        private Unit source;
        public MoveAroundAndChill(Unit source) 
        {
            this.source = source;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new UnitMoveAction(source, RNG.RandomPointWithinCircleRelativeToLocation(source.Location, new GLCoordinate(0.4f, 0.4f))),
                //new HeroMoveAction(new GLCoordinate(0.2f, 0.2f), source),
                //new MoveAction(RNG.RandomPointWithinCircleRelativeToLocation(source.Location, new GLCoordinate(0.4f, 0.4f)), source),
                new ChillAction(),
            };
        }
    }

    class NeverEndingPatrol : ActionPattern
    {
        private Unit source;
        private GameCoordinate patrolDelta;
        public NeverEndingPatrol(Unit source, GameCoordinate patrolDelta)
        {
            this.source = source;
            this.patrolDelta = patrolDelta;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new NeverEndingPatrolAction(source, patrolDelta),
                new ChillAction(),
            };
        }
    }


    /// <summary>
    /// this one is a piece of shit.
    /// location is stored at the init and is not updated when the shitter moves.
    /// This means that the programmer needs to keep track of where the entity is located and move it back accordingly
    /// todo: make it possible for actions to be initialized when the previous one has finished execution.
    /// </summary>
    class PatrolAndChill : ActionPattern
    {
        private Unit source;
        private GameCoordinate patrolDelta;
        public PatrolAndChill(Unit source, GameCoordinate patrolDelta)
        {
            this.source = source;
            this.patrolDelta = patrolDelta;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new MoveAction(new GameCoordinate(source.Location.X + patrolDelta.X, source.Location.Y + patrolDelta.Y), source),
                new ChillAction(),
                new MoveAction(new GameCoordinate(source.Location.X - patrolDelta.X, source.Location.Y - patrolDelta.Y), source),
                new ChillAction(),
            };
        }
    }
}
