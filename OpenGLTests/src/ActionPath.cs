using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class PlacedActions
    {
        LinkedList<GameAction> linkedList = new LinkedList<GameAction>();

        public void Add(GameAction placed, IActionCapable owner)
        {
            linkedList.AddLast(placed);
            
            //create line between previous set marker and newly placed marker
            //MarkerLine ml = new MarkerLine(linkedList, owner);
            //placed.Marker.SetMarkerLine(ml);
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

        public GameAction LastMoveAction()
        {
            var node = linkedList.Last;

            while (node != null)
            {
                //make interface for all move kinds
                if (node.Value.Marker is MoveMarker) return node.Value;
                node = node.Previous;
            }

            return null;
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
        private Button CurrentActionButton { get; set; }
        private GameAction previousGameAction { get; set; }
        private GameAction CurrentAction { get; set; }
        private IActionCapable owner;
        private CombatActionHandler combatActionHandler { get; set; }
        private OutOfCombatActionHandler outOfCombatActionHandler { get; set; }

        public ActionHandler(IActionCapable owner)
        {
            //save owner
            this.owner = owner;

            //init button
            CurrentActionButton = new Button();
            CurrentActionButton.Size = new GLCoordinate(0.2f, 0.2f);
            CurrentActionButton.Location = new GLCoordinate(-0.9f, -0.9f);
            CurrentActionButton.Color = Color.NavajoWhite;
            GameState.Drawables.Add(CurrentActionButton);

            //init handlers
            combatActionHandler = new CombatActionHandler();
            outOfCombatActionHandler = new OutOfCombatActionHandler();
        }

        /// <summary>
        /// Called from an inventory button or an action bar button
        /// </summary>
        /// <param name="ab"></param>
        public void ActionButtonClicked(ActionButton ab)
        {
            //Updates the button indicating which ability is currently active.
            ab.Animation.GetSprite();
            CurrentActionButton.Animation = ab.Animation;
            CurrentActionButton.Animation.SetSprite(ab.Animation.GetSprite().sid);

            //Update current actions
            previousGameAction = CurrentAction;
            CurrentAction = ab.GameAction;
        }

        /// <summary>
        /// Called from Screen when mouse has been clicked down.
        /// </summary>
        /// <param name="location"></param>
        public void Down(GameCoordinate location)
        {
            if (CurrentAction == null) return;
            CurrentAction.RangeShape.Visible = true;
            CurrentAction.Marker.Visible = true;

            if (owner.InCombat)
            {
                var origin = combatActionHandler.GetOriginOfLastPlacedAction();
                if (origin != null) CurrentAction.RangeShape.Location = previousGameAction.Marker.Location;
            }

        }

        /// <summary>
        /// Called from Screen when mouse has been released.
        /// </summary>
        /// <param name="location"></param>
        public void Up(GameCoordinate location)
        {
            if (CurrentAction == null) return;
            CurrentAction.RangeShape.Visible = false;

            if (CurrentAction.RangeShape.Contains(location) || CurrentAction.RangeShape.IsInfinite)
            {
                if (CurrentAction.Ready)
                {
                    if (owner.InCombat)
                    {
                        combatActionHandler.PlaceAction(CurrentAction, location, owner);
                    }
                }
                //if in combat -> queue
                //else do it(unless its placeable)
            }
        }

        public ActionReturns TryInvokeCurrentAction(object args)
        {
            if (CurrentAction.Ready)
            {
                bool finished = CurrentAction.GetAction().Invoke(args);
                if (finished) return ActionReturns.Finished;
                return ActionReturns.Ongoing;
            }
            else return ActionReturns.NotReady;
        }
    }

    public class CombatActionHandler
    {
        private PlacedActions placedActions;
        public CombatActionHandler()
        {
            placedActions = new PlacedActions();
        }

        public void PlaceAction(GameAction action, GameCoordinate location, IActionCapable owner)
        {
            //set the location of the marker to location as well as the lines terminus to location
            action.Marker.UpdatePositionOfLineAndMarker(location);

            var lastPlacedAction = placedActions.LastMoveAction();

            if (lastPlacedAction != null)
            {
                action.Marker.SetLineOrigin(lastPlacedAction.Marker.Location);
            }
            else
            {
                action.Marker.SetLineOrigin(owner.Location);
            }


            placedActions.Add(action, owner);
        }

        public GameCoordinate GetOriginOfLastPlacedAction()
        {
            var lma = placedActions.LastMoveAction();

            if (lma == null) return null;
            else return lma.Marker.Location;
        }
    }

    public class OutOfCombatActionHandler
    {

    }
}
