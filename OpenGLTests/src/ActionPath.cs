using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public class MarkerLine : Drawable
    {
        private Line line;
        private GameCoordinate terminus;
        private GameCoordinate origin;

        public MarkerLine(LinkedList<GameAction> actions, IActor owner)
        {
            var node = actions.Last;
            while (node != null)
            {
                if (terminus == null)
                {
                    terminus = node.Value.Marker.Location;
                }
                else if (node.Value.Marker is MoveMarker)
                {
                    origin = node.Value.Marker.Location;
                    break; //donezo
                }
                node = node.Previous;
            }

            if (origin == null) origin = owner.Location;

            if (actions.Last.Value.Marker is MoveMarker)
            {
                line = new SolidLine(terminus, origin);
            }
            else if (actions.Last.Value.Marker is ActionMarker)
            {
                line = new DashedLine(terminus, origin);
            }
            else //default
            {
                line = new SolidLine(terminus, origin);
            }
        }

        public override void Draw(DrawAdapter drawer)
        {
            line.Draw(drawer);
        }
    }

    public class PlacedActions
    {
        LinkedList<GameAction> linkedList = new LinkedList<GameAction>();

        public void Add(GameAction placed, IActor owner)
        {
            //add a new marker, both for drawing and in linked list
            GameState.Drawables.Add(placed.Marker);
            linkedList.AddLast(placed);

            //create line between previous set marker and newly placed marker
            MarkerLine ml = new MarkerLine(linkedList, owner);
            placed.Marker.SetMarkerLine(ml);
        }

        public void Remove(GameAction action)
        {
            var node = linkedList.First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value == action)
                {
                    linkedList.Remove(node);
                    GameState.Drawables.Remove(node.Value.Marker);
                    node = nextNode;
                    while (node != null)
                    {
                        var temp = node;
                        node = node.Next;
                        linkedList.Remove(temp);
                        GameState.Drawables.Remove(temp.Value.Marker);
                    }
                }
                node = nextNode;
            }
        }

        public LinkedList<GameAction> Get()
        {
            return linkedList;
        }

        public GameAction Last()
        {
            if (linkedList.Count == 0) return null;
            return linkedList.Last();
        }

        public GameAction First()
        {
            if (linkedList.Count == 0) return null;
            return linkedList.First();
        }

        public void RemoveFirst()
        {
            var node = linkedList.First;
            GameState.Drawables.Remove(node.Value.Marker);
            linkedList.Remove(node);
        }
    }

    public class CombatActionHandler
    {
        private GameAction activeAction { get; set; }
        private GameAction previousActiveAction { get; set; }
        private List<GameAction> availableActions = new List<GameAction>();
        public PlacedActions PlacedActions = new PlacedActions();

        private IActor owner;

        public CombatActionHandler(IActor owner)
        {
            this.owner = owner;

        }

        /// <summary>
        /// If active action is already placed update the actions targeted location.
        /// Else enqueue the action at the targeted position.
        /// </summary>
        /// <param name="position"></param>
        public void EnqueueAction(GameCoordinate position)
        {
            activeAction.Marker.Location = position;
            if (!PlacedActions.Get().ToList().Contains(activeAction))
            {
                PlacedActions.Add(activeAction, owner);
            }

            /* this adds lines between all placed actions, although there is now way to remove them later.
            GameCoordinate origin = owner.Location;
            var moves = PlacedActions.Get().ToList().Where(e => e.Marker is MoveMarker);
            foreach (var pa in moves)
            {
                GameCoordinate terminus = pa.Marker.Location;
                GameState.AddDrawable(new Line(origin, terminus));
                origin = terminus;
            }*/
        }
        /// <summary>
        /// Add a new game action to the owners available actions.
        /// </summary>
        /// <param name="a"></param>
        public void AddNewAvailableAction(GameAction a)
        {
            availableActions.Add(a);
        }
        /// <summary>
        /// Returns the active action. If no action has been set to active: set the first available action to active and return it.
        /// </summary>
        /// <returns></returns>
        public GameAction GetActiveAction()
        {
            if(activeAction == null) SetActiveAction(availableActions.First());
            return activeAction;
        }

        /// <summary>
        /// Sets the active actions range shape location to the last placed move marker. If no move marker has been placed: the location of the owner.
        /// Then returns the active actions range shape.
        /// </summary>
        /// <returns>Then returns the active actions range shape.</returns>
        public RangeShape GetActiveRangeShape()
        {
            GetActiveAction().RangeShape.Location = getOriginOfRangeShape();
            return activeAction.RangeShape;
        }

        /// <summary>
        /// Returns the origin of the last placed move marker. If no move marker has been placed returns the location of the owner.
        /// </summary>
        /// <returns></returns>
        private GameCoordinate getOriginOfRangeShape()
        {
            GameCoordinate location = owner.Location;
            if (PlacedActions.Last() != null)
            {
                var pas = PlacedActions.Get().ToList();
                var xd = pas.Where(a => a != activeAction && a.Marker is MoveMarker);
                if (xd.Any())
                {
                    location = xd.Last().Marker.Location;
                }
            }

            return location;
        }

        /// <summary>
        /// Removes all placed actions which has been placed after the active action.
        /// </summary>
        public void RemoveAllPlacedAfterActive()
        {
            PlacedActions.Remove(activeAction);
        }

        /// <summary>
        /// Set the active action to the received action. This will update the previous action the action that was active before the change.
        /// </summary>
        /// <param name="a"></param>
        public void SetActiveAction(GameAction a)
        {
            previousActiveAction = activeAction;
            activeAction = a;
            if (previousActiveAction != null)
            {
                GameState.Drawables.Remove(previousActiveAction.RangeShape);
            }
            GameState.Drawables.Add(activeAction.RangeShape);
        }


        public ActionReturns TickPlacedActions(int index)
        {
            //get first action that has not been executed. if this action is null all actions have been executed.
            var firstAction = PlacedActions.First();
            if (firstAction == null) return ActionReturns.AllFinished;

            //call action with index. if action call returns true this specific action is Finished executing.
            bool actionFinished = firstAction.GetAction().Invoke(index);
            if (actionFinished)
            {
                PlacedActions.RemoveFirst();
                return ActionReturns.Finished;
            }

            return ActionReturns.Ongoing;
        }

        public void DoActiveAction(GameCoordinate clicked)
        {
            activeAction.Marker.Location = clicked;
            activeAction.GetAction().Invoke(2);
        }
    }

    public class OutOfCombatActionHandler
    {
        private GameAction currentAction;
        private object args;

        public void SetCurrentAction(GameAction action, object args)
        {
            this.currentAction = action;
            this.args = args;
        }

        public bool DoGameAction()
        {
            if (currentAction == null) return false;

            var res = currentAction.GetAction().Invoke(args);
            if (res)
            {
                currentAction = null;
            }

            return res;
        }
    }
}
