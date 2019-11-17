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
    /// Contains a list of game actions that will be executed in turn and returns the game actions execution status.
    /// Execution statuses can be: AllFinished, Finished, Ongoing, Placing, .. And are enumerated in "ActionReturns"
    /// </summary>
    public abstract class ActionPattern
    {
        private List<GameAction> actions;
        public List<GameAction> Actions
        {
            get { return actions; }
            set
            {
                actions = value;
                foreach (var a in value)
                {
                    ActionsUsed.Add(a, false);
                }
            }
        }

        public bool Loop = false;
        
        public Dictionary<GameAction, bool> ActionsUsed = new Dictionary<GameAction, bool>();

        public virtual ActionReturns DoAction(object arg)
        {
            if(ActionsUsed.All(a => a.Value == true))
            {
                if (Loop)
                {
                    ActionsUsed.ToList().ForEach(a => ActionsUsed[a.Key] = false);
                }
                return ActionReturns.AllFinished;
            }

            var currentGameAction = ActionsUsed.First(a => a.Value == false);
            if((int)arg == 0) currentGameAction.Key.PlacedLocation = currentGameAction.Key.NPCActionPlacementCalculator(NPCState.Angry);
            var actionFinished = currentGameAction.Key.GetAction().Invoke(arg);
            if (actionFinished)
            {
                ActionsUsed[currentGameAction.Key] = true;
                return ActionReturns.Finished;
            }
            else
            {
                return ActionReturns.Ongoing;
            }
        }

        public void Dispose()
        {
            foreach (var a in ActionsUsed.Keys)
            {
                if(a.Marker != null) a.Marker.Dispose();
                if(a.ActionLine != null) a.ActionLine.Dispose();
                GameState.Drawables.Remove(a.RangeShape);
            }
        }
    }

    class TailoredPattern : ActionPattern
    {
        public TailoredPattern(params GameAction[] actions)
        {
            this.Actions = actions.ToList();
        }
    }

    class StitchedPattern : ActionPattern
    {
        public StitchedPattern(params ActionPattern[] patterns)
        {
            List<GameAction> actions = new List<GameAction>();
            foreach (var pattern in patterns)
            {
                actions.AddRange(pattern.Actions);
            }

            Actions = actions;
        }
    }

    class SwamperTeleportPattern : ActionPattern
    {
        public SwamperTeleportPattern(Unit source, GLCoordinate range)
        {
            Actions = new List<GameAction>()
            {
                new InstantTeleport(RNG.RandomPointWithinCircleRelativeToLocation(source.Location, range), source)
            };
        }
    }

    class FindAndChaseEntity : ActionPattern
    {
        private Unit chasing;

        public FindAndChaseEntity(Unit source, RangeShape chasingShape)
        {
            Actions = new List<GameAction>()
            {
                new FindAndChase(chasingShape, source),
            };
        }
    }

    class FindAndFleeEntity : ActionPattern
    {
        private Unit fleeing;

        public FindAndFleeEntity(Unit source, RangeShape chasingShape)
        {
            Actions = new List<GameAction>()
            {
                new FindAndFlee(chasingShape, source),
            };
        }
    }

    class MoveAroundAndChill : ActionPattern
    {
        public MoveAroundAndChill(Unit source) 
        {
            Actions = new List<GameAction>()
            {
                new UnitMoveAction(source, source.Location),
                new ChillAction(),
            };
        }
    }

    class NeverEndingPatrol : ActionPattern
    {
        public NeverEndingPatrol(Unit source, GameCoordinate patrolDelta)
        {
            Actions = new List<GameAction>()
            {
                new NeverEndingPatrolAction(source, patrolDelta),
                new ChillAction(),
            };
        }
    }
}
