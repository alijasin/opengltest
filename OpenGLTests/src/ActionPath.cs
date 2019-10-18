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
using OpenTK.Graphics.ES10;

//this is a mess. Todo: refactor action handlers.
//Have OutOFCombat action handler and CombatActionHandler inherit from ActionHandler. Then depending on whether owner is in combat utilize the correct one.
//Commonalities: Up, Down, Queue Action, Execution actions.
namespace OpenGLTests.src
{
    public class PlacedActions
    {
        LinkedList<GameAction> linkedList = new LinkedList<GameAction>();

        public void Add(GameAction placed)
        {
            linkedList.AddLast(placed);
        }

        public void Clear()
        {
            linkedList.Clear();
        }

        public int CountExceptThisType(Type exceptionType)
        {
            int c = 0;

            var node = linkedList.First;
            while (node != null)
            {
                if (node.Value.GetType() != exceptionType) c++;
                node = node.Next;
            }

            return c;
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
                if (node.Value.GetType() == pga.GetType())
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

    public interface IActionHandler
    {
        IActionCapable Owner { get; set; }
        GameAction SelectedAction { get; set; }
        void TryPlaceAction(GameAction action, GameCoordinate location);
        ActionReturns CommitActions(object args);
        void OnMouseUp(GameCoordinate mouseLocation);
        void OnMouseDown(GameCoordinate mouseLocation);
        void ActionButtonClicked(ActionButton actionButton);
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
            //action.Place(location);
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

    public class OutOfCombatPlacedActions : List<GameAction>
    {
        public void RemoveWhere(Func<GameAction, bool> filter)
        {
            List<GameAction> toRemove = new List<GameAction>();
            foreach (var pa in this)
            {
                if (filter(pa))
                {
                    pa.Dispose();
                    toRemove.Add(pa);
                }
            }

            this.RemoveAll(pa => toRemove.Contains(pa));
        }

        public void RemoveGameAction(GameAction ga)
        {
            ga.Dispose();
            this.Remove(ga);
        }

        public List<GameAction> GetWhere(Func<GameAction, bool> filter)
        {
            List<GameAction> matching = new List<GameAction>();
            foreach (var pa in this)
            {
                if(filter(pa)) matching.Add(pa);
            }

            return matching;
        }
    }

    public class OutOfCombatActionHandler : IActionHandler
    {
        private static int i = 0;
        OutOfCombatPlacedActions PlacedActions = new OutOfCombatPlacedActions();
        public IActionCapable Owner { get; set; }
        public GameAction SelectedAction { get; set; }
        private SpriteID SelectedActionIcon { get; set; }

        public OutOfCombatActionHandler(IActionCapable owner)
        {
            this.Owner = owner;

        }

        public void TryPlaceAction(GameAction action, GameCoordinate location)
        {
            //If clicked within range or if the range is infinite
            i++;
            Console.WriteLine(i);
            if (action.RangeShape.IsInfinite || action.RangeShape.Contains(location))
            {
                Console.WriteLine("Placed " + action);

                //remove all placed actions that are identical to the new one.
                PlacedActions.RemoveWhere(pa => pa.GetType() == action.GetType());

                action.Place(location, SelectedActionIcon);
                PlacedActions.Add(action);
            }
            else
            {
                Console.WriteLine("Did not place " + action);
            }
        }

        public void OnMouseUp(GameCoordinate mouseLocation)
        {
            if(SelectedAction == null) return;

            SelectedAction.RangeShape.Visible = false;
            TryPlaceAction(SelectedAction, mouseLocation);
        }

        public void OnMouseDown(GameCoordinate mouseLocation)
        {
            if (SelectedAction == null) return;

            SelectedAction.RangeShape.Visible = true;

        }

        public void ActionButtonClicked(ActionButton actionButton)
        {
            SelectedAction = actionButton.GameAction;
            SelectedActionIcon = actionButton.Animation.GetSprite().sid;
            Console.WriteLine("Selected action: " + SelectedAction);
        }

        public ActionReturns CommitActions(object args)
        {
            if (PlacedActions.Count == 0) return ActionReturns.NoAction;

            var currentAction = PlacedActions.First();

            var finished = currentAction.GetAction().Invoke(args);
            if (finished)
            {
                PlacedActions.RemoveGameAction(currentAction);
                return ActionReturns.Finished;
            }
            else if (currentAction.IsPlaced == false) return ActionReturns.Placing;
            return ActionReturns.Ongoing;
        }

    }
}
