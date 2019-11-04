using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;
using Console = System.Console;

namespace OpenGLTests.src
{
    /// <summary>
    /// Contains a list of game actions that will be executed and returns the execution status.
    /// </summary>
    public abstract class ActionPattern
    {
        public abstract List<GameAction> InitPattern();
        public List<GameAction> Actions = new List<GameAction>();
        public bool Loop = false;

        public virtual ActionReturns DoAction(object arg)
        {
            if (Actions.Count == 0)
            {
                //reinitialize the pattern - all randomness will be regenerated.
                if (Loop)
                {
                    Actions = InitPattern();
                }
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
        List<ActionPattern> initialPatterns = new List<ActionPattern>();
        public CustomPattern(params ActionPattern[] patterns)
        {
            initialPatterns = patterns.ToList();
        }
        public override List<GameAction> InitPattern()
        {
            List<GameAction> actions = new List<GameAction>();
            foreach (var pattern in initialPatterns)
            {
                actions.AddRange(pattern.InitPattern());
            }

            return actions;
        }
    }

    class DebugPattern : ActionPattern
    {
        private Unit source;
        public DebugPattern(Unit source)
        {
            this.source = source;
            InitPattern();
        }

        public override List<GameAction> InitPattern()
        {
            var actions = new List<GameAction>()
            {
                new LambdaAction(o =>
                {
                    //Console.WriteLine(RNG.IntegerBetween(419, 421));
                    return true;

                }, source),
            };
            return actions;
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

        public override List<GameAction> InitPattern()
        {
            var actions = new List<GameAction>()
            {
                new InstantTeleport(RNG.RandomPointWithinCircleRelativeToLocation(source.Location, range), source),
            };
            return actions;
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

        public override List<GameAction> InitPattern()
        {
            var actions = new List<GameAction>()
            {
                new FindAndChase(new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), source), source),
            };
            return actions;
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

        public override List<GameAction> InitPattern()
        {
            var actions = new List<GameAction>()
            {
                new FindAndFlee(new RangeShape(new Circle(new GLCoordinate(0.2f, 0.2f)), source), source),
            };
            return actions;
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

        public override List<GameAction> InitPattern()
        {
            var actions = new List<GameAction>()
            {
                new UnitMoveAction(source, RNG.RandomPointWithinCircleRelativeToLocation(source.Location, new GLCoordinate(0.4f, 0.4f))),
                new ChillAction(),
            };
            return actions;
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

        public override List<GameAction> InitPattern()
        {
            var actions = new List<GameAction>()
            {
                new NeverEndingPatrolAction(source, patrolDelta),
                new ChillAction(),
            };
            return actions;
        }
    }
}
