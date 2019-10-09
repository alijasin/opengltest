using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public class PlacedActionOrderedList
    {
        LinkedList<GameAction> linkedList = new LinkedList<GameAction>();

        public void Add(GameAction placed)
        {
            GameState.Drawables.Add(placed.RangeShape.Marker);
            linkedList.AddLast(placed);
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
                    node = nextNode;
                    while (node != null)
                    {
                        var temp = node;
                        node = node.Next;
                        linkedList.Remove(temp);
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

        public void Clear()
        {
            List<ActionMarker> markers = new List<ActionMarker>(GameState.Drawables.Where(e => e is ActionMarker).Cast<ActionMarker>());
            foreach (var m in markers)
            {
                GameState.Drawables.Remove(m);
            }
            linkedList.Clear();
        }
    }

    public class ActionHandler
    {
        private GameAction activeAction { get; set; }
        private GameAction previousActiveAction { get; set; }
        private ActionBar actionBar { get; set; }
        private List<GameAction> availableActions = new List<GameAction>();
        public PlacedActionOrderedList PlacedActions = new PlacedActionOrderedList();
        private IActor owner;
        public ActionHandler(IActor owner)
        {
            this.owner = owner;
            actionBar = new ActionBar(owner);
            GameState.Drawables.Add(actionBar);
        }

        /// <summary>
        /// If active action is already placed update the actions targeted location.
        /// Else enqueue the action at the targeted position.
        /// </summary>
        /// <param name="position"></param>
        public void EnqueueAction(GameCoordinate position)
        {
            if (PlacedActions.Get().ToList().Contains(activeAction))
            {
                activeAction.RangeShape.Marker.Location = position;
            }
            else
            {
                activeAction.RangeShape.Marker = new ActionMarker(position);
                PlacedActions.Add(activeAction);
            }
        }

        public void AddNewAvailableAction(GameAction a)
        {
            availableActions.Add(a);

            var actionButton = new ActionButton(a);
            actionBar.Add(actionButton);
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
        /// Sets the active actions range shape to the last placed action's marker, and in the case that no marker has been placed: the owner's location.
        /// Then returns the active actions range shape.
        /// </summary>
        /// <returns>Then returns the active actions range shape.</returns>
        public RangeShape GetActiveRangeShape()
        {
            GameCoordinate location = owner.Location;
            if (PlacedActions.Last() != null)
            {
                var pas = PlacedActions.Get().ToList();
                var xd = pas.Where(a => a != activeAction);
                if (xd.Any())
                {
                    location = pas.Last(a => a != activeAction).RangeShape.Marker.Location;

                }

            }

            GetActiveAction().RangeShape.Location = location;
            return activeAction.RangeShape;
        }

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

        public void DoCommitedActions()
        {
            var placed = PlacedActions.Get().ToList();
            foreach (var ca in placed)
            {
                Console.WriteLine("invoking " + ca);
                ca.GetAction().Invoke();
                PlacedActions.Remove(ca);
                GameState.Drawables.Remove(ca.RangeShape.Marker);
            }
            PlacedActions.Clear();
        }
    }
}
