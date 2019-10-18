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
using OpenGLTests.src.Util;

//this is a mess. Todo: refactor action handlers
namespace OpenGLTests.src
{
    public class PlacedActions
    {
        LinkedList<GameAction> linkedList = new LinkedList<GameAction>();

        public void Add(GameAction placed)
        {
            linkedList.AddLast(placed);
        }

        public void Remove(GameAction action)
        {
            var node = linkedList.Last;

            while (node != null)
            {
                if (node.Value == action)
                {
                    node.Value.Dispose();
                    linkedList.Remove(node);
                    return;
                }

                var prev = node.Previous;

                node.Value.Dispose();
                linkedList.Remove(node);
                node = prev;
            }
        }

        public GameAction LastMoveAction()
        {
            var node = linkedList.Last;

            while (node != null)
            {
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
            node.Value.Dispose();
            linkedList.Remove(node);
        }

        //todo compares by action instead of unique way. this will prevent duplicated pga.
        public bool AlreadyPlaced(GameAction pga)
        {
            var node = linkedList.First;
            while (node != null)
            {
                if (node.Value.GetAction() == pga.GetAction())
                {
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public int Count()
        {
            return linkedList.Count;
        }

        /// <summary>
        /// Todo: This just returns a constant. In the future there should be some kind of placed action panel that can be hovered or similiar.
        ///  Not just paint it at a fixed location.
        /// </summary>
        /// <param name="lpa"></param>
        /// <returns></returns>
        public GameCoordinate InstantLocationOffset(GameAction lpa)
        {
            var node = linkedList.First;
            GameCoordinate offset = new GameCoordinate(0f, -lpa.Marker.Size.Y);
            while (node != null)
            {
                if (node.Value == lpa)
                {
                    //if(lpa.Marker.)
                }

                node = node.Next;
            }

            return offset;
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

            if (owner.InCombat)
            {
                combatActionHandler.ClearPreviouslyPlacedBefore(CurrentAction);
                var origin = combatActionHandler.GetOriginOfLastPlacedAction();
                if (origin == null || previousGameAction == null) CurrentAction.RangeShape.Location = owner.Location;
                else CurrentAction.RangeShape.Location = origin;

                if(CurrentAction.IsInstant) combatActionHandler.PlaceAction(CurrentAction, owner.Location, owner, CurrentActionButton.Animation.GetSprite().sid, true);
            }
            else
            {
                //outOfCombatActionHandler.SetAction()
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
                        combatActionHandler.PlaceAction(CurrentAction, location, owner, CurrentActionButton.Animation.GetSprite().sid);
                        //CurrentAction.SetMarkerIcon(CurrentActionButton.Animation.GetSprite().sid);
                    }
                }
                //if in combat -> queue
                //else do it(unless its placeable)
            }
        }

        public ActionReturns TryInvokePlacedActions(object args)
        {
            ActionReturns status = combatActionHandler.DoAction(args);
            return status;
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
        
        public void PlaceAction(GameAction action, GameCoordinate location, IActionCapable owner, SpriteID sid, bool instant = false)
        {
            action.Marker.Location = location;
            var lastMoveAction = placedActions.LastMoveAction();

            GameCoordinate lineOrigin = location;
            GameCoordinate lineTerminus;

            if (lastMoveAction != null)
            {
                lineTerminus = lastMoveAction.Marker.Location;
                if (instant)
                {
                    lineOrigin = lastMoveAction.Marker.Location;
                    action.Marker.Location = lastMoveAction.Marker.Location + placedActions.InstantLocationOffset(lastMoveAction);
                }
            }
            else
            {
                lineTerminus = owner.Location;
            }

            action.PositionLine(lineOrigin, lineTerminus);

            action.SetMarkerIcon(sid);
            action.Place();
            placedActions.Add(action);
        }

        public GameCoordinate GetOriginOfLastPlacedAction()
        {
            var lma = placedActions.LastMoveAction();

            if (lma == null) return null;
            else return lma.Marker.Location;
        }

        public void ClearPreviouslyPlacedBefore(GameAction currentAction)
        {
            if (placedActions.AlreadyPlaced(currentAction))
            {
                placedActions.Remove(currentAction);
            }
        }

        public ActionReturns DoAction(object arg)
        {
            bool completed = placedActions.First().GetAction().Invoke(arg);
            if (completed)
            {
                placedActions.RemoveFirst();
            }
            else return ActionReturns.Ongoing;

            if (placedActions.Count() == 0) return ActionReturns.AllFinished;
            else return ActionReturns.Finished;

        }
    }

    public class OutOfCombatActionHandler
    {

    }
}
