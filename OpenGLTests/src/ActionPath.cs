using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;

//this is a mess. Todo: refactor action handlers
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

        public override void DrawStep(DrawAdapter drawer)
        {
            line.DrawStep(drawer);
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

    public class ActionHandler
    {
        public CombatActionHandler CombatActionHandler { get; set; }
        public OutOfCombatActionHandler OutOfCombatActionHandler { get; set; }
        public GameAction activeAction { get; set; }
        public IPlaceable nonPlacedActiveAction { get; set; }
        public IActor owner { get; set; }

        public ActionHandler(IActor owner)
        {
            OutOfCombatActionHandler = new OutOfCombatActionHandler(this);
            CombatActionHandler = new CombatActionHandler(this);
            this.owner = owner;
        }

        public void ClearActiveAction()
        {
            activeAction.Dispose();
            activeAction = null;
        }

        public void SetActiveAction(GameAction action)
        {
            var previousAction = activeAction;
            if (previousAction != null)
            {
                if(previousAction.RangeShape != null)
                previousAction.RangeShape.Visible = false;
            }

            activeAction = action;
            activeAction.RangeShape.Visible = true;
            GameState.Drawables.Add(activeAction.RangeShape);
        }

        public void Clicked(IPlaceable placeable)
        {
            placeable.Clicked();
            nonPlacedActiveAction = placeable;
        }
    }

    public class CombatActionHandler
    {
        public PlacedActions PlacedActions = new PlacedActions();

        private ActionHandler handler;
        private GameAction activeAction
        {
            get { return handler.activeAction; }
            set { handler.activeAction = value; }
        }
        private IActor owner => handler.owner;

        public CombatActionHandler(ActionHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// If active action is already placed update the actions targeted location.
        /// Else enqueue the action at the targeted position.
        /// </summary>
        /// <param name="position"></param>
        public void TryEnqueueAction(GameCoordinate position)
        {
            if (activeAction?.RangeShape == null) return;

            activeAction.RangeShape = GetActiveRangeShape();
            activeAction.Marker.Location = position;
            activeAction.RangeShape.Visible = true;


            if (activeAction.RangeShape.Contains(position))
            {
                if (!PlacedActions.Get().ToList().Contains(activeAction))
                {
                    PlacedActions.Add(activeAction, owner);
                }
            }
            
        }

        /// <summary>
        /// Returns the active action. If no action has been set to active: set the first available action to active and return it.
        /// </summary>
        /// <returns></returns>
        public GameAction GetActiveAction()
        {
            return activeAction;
        }

        /// <summary>
        /// Sets the active actions range shape location to the last placed move marker. If no move marker has been placed: the location of the owner.
        /// Then returns the active actions range shape.
        /// </summary>
        /// <returns>Rreturns the active actions range shape centered at last placed action's location or hero's location if no placed actions.</returns>
        public RangeShape GetActiveRangeShape()
        {
            if (GetActiveAction() == null) return null;
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


        public ActionReturns TickPlacedActions(int index)
        {
            //get first action that has not been executed
            var firstAction = PlacedActions.First();

            //call action with index. if action call returns true this specific action is Finished executing.
            bool actionFinished = firstAction.GetAction().Invoke(index);
            if (actionFinished)
            {
                PlacedActions.RemoveFirst();
                if (PlacedActions.First() == null)
                {
                    handler.ClearActiveAction();
                    return ActionReturns.AllFinished;
                }
                return ActionReturns.Finished;
            }

            return ActionReturns.Ongoing;
        }

        public void Down()
        {
            if (activeAction?.RangeShape == null) return;
            setRangeShapeVisibility(true);

            RemoveAllPlacedAfterActive();
        }

        private void setRangeShapeVisibility(bool visible)
        {
            activeAction.RangeShape = GetActiveRangeShape();
            activeAction.RangeShape.Visible = visible;
        }
    }

    public class OutOfCombatActionHandler
    {
        private ActionHandler handler;
        
        private GameAction activeAction
        {
            get { return handler.activeAction; }
            set { handler.activeAction = value; }
        }
        private IActor owner => handler.owner;

        public OutOfCombatActionHandler(ActionHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// If no active action - make a move action.
        /// If active action exists but it is a move action - replace it with move action.
        /// Else call the iplaceable placed function of the active action.
        /// </summary>
        /// <param name="pos"></param>
        public void Placed(GameCoordinate pos)
        {
            if (handler.nonPlacedActiveAction != null)
            {
                bool placed = handler.nonPlacedActiveAction.Placed(pos);
                if (placed)
                {
                    activeAction = (GameAction) handler.nonPlacedActiveAction;
                    handler.nonPlacedActiveAction = null;
                }
            }
            else if (activeAction != null)
            {
                if (activeAction.GetType() == typeof(MoveTowardsAction))
                {
                    activeAction = new MoveTowardsAction(pos, (Entity)owner);
                }
                else
                {
                }
            }
            else
            {
                activeAction = new MoveTowardsAction(pos, (Entity)owner);
            }
        }

        public ActionReturns TickGameAction(int index)
        {
            if (activeAction == null) return ActionReturns.Finished;

            bool actionFinished = activeAction.GetAction().Invoke(index);
            if (actionFinished)
            {
                handler.ClearActiveAction();
                return ActionReturns.Finished;
            }

            return ActionReturns.Ongoing;
        }
    }
}
