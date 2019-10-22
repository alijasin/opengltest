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
    abstract class ActionPattern
    {
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
                GameState.Drawables.Remove(a.Marker);
                GameState.Drawables.Remove(a.RangeShape);
            }
        }
    }

    class TeleportPattern : ActionPattern
    {
        private GLCoordinate range;
        private ICombatable source;
        public TeleportPattern(ICombatable source, GLCoordinate range)
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

    class ChaseEntity : ActionPattern
    {
        private ICombatable source;
        private object argument;

        public ChaseEntity(ICombatable source, ICombatable chasing)
        {
            this.source = source;

            argument = (Func<GameCoordinate>)(() =>
            {
                return chasing.Location;
            });

            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new MoveTowardsEntityAction(source)

            };
        }

        public override ActionReturns DoAction(object arg)
        {
            return base.DoAction(argument);
        }
    }

    class MoveAroundAndChill : ActionPattern
    {
        private ICombatable source;
        public MoveAroundAndChill(ICombatable source) 
        {
            this.source = source;
            InitPattern();
        }

        public override void InitPattern()
        {
            Actions = new List<GameAction>()
            {
                new MoveAction(RNG.RandomPointWithinCircleRelativeToLocation(source.Location, new GLCoordinate(0.4f, 0.4f)), source),
                new ChillAction(),
            };
        }
    }

    class NeverEndingPatrol : ActionPattern
    {
        private ICombatable source;
        private GameCoordinate patrolDelta;
        public NeverEndingPatrol(ICombatable source, GameCoordinate patrolDelta)
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
        private ICombatable source;
        private GameCoordinate patrolDelta;
        public PatrolAndChill(ICombatable source, GameCoordinate patrolDelta)
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
