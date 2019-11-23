using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Screens;
using OpenGLTests.src.Util;
using OpenTK;
using OpenTK.Graphics.ES10;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class SubsequentlyPlacedActions
    {
        LinkedList<GameAction> linkedList = new LinkedList<GameAction>();

        public void Add(GameAction placed)
        {
            linkedList.AddLast(placed);
        }

        public void Clear()
        {
            foreach (var ga in linkedList)
            {
                ga.Dispose();
            }
            linkedList.Clear();
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

        public GameAction First()
        {
            if (linkedList.Count == 0) return null;
            return linkedList.First();
        }

        public void RemoveFirst()
        {
            var node = linkedList.First;
            if (node == null) return;
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
    }

    public abstract class ActionHandler
    {
        public ActionButton CurrentButtonSelected { get; set; }
        public IActionCapable Owner { get; set; }
        protected Action onFinishedCasting { get; set; } = () => { };
        protected Action onChangedAction { get; set; } = () => { };
        private GameAction selectedAction;
        public GameAction SelectedAction
        {
            get { return selectedAction; }
            set
            {
                if (selectedAction != null)
                {
                    onChangedAction();
                    selectedAction.Hide();
                }


                selectedAction = value;
                selectedAction.Show();
                //check not needed after Hero has been refactored into player. TODO
                if(Player.Cursor != null) Player.Cursor.SetAction(value);
            }
        }

        public ActionHandler(IActionCapable owner)
        {
            Owner = owner;
        }

        public abstract ActionStatus CommitActions(object args);

        public void OnMouseUp(GameCoordinate mouseLocation)
        {
            if (SelectedAction == null) return;
            if (Owner.ActionStatus == ActionStatus.WaitingForOther) return;

            if (SelectedAction.PlacementFilter(mouseLocation))
            {
                PlaceAction(SelectedAction, mouseLocation);
                if (SelectedAction.IsPlaced)
                {
                    Player.Cursor.Hide();
                }
            }

            SelectedAction.RangeShape.Visible = false;
        }

        public abstract void OnMouseDown(GameCoordinate mouseLocation);

        public void ActionButtonActivated(ActionButton bu)
        {
            var oldSelected = SelectedAction;
            if(oldSelected != null) oldSelected.Dispose();
            SelectedAction = bu.GameAction;
            CurrentButtonSelected = bu;

            if (bu is InventorySlot islot)
            {
                onFinishedCasting = () => { islot.Consume(); };
            }

            if (bu is EquipmentSlot eslot)
            {
                onChangedAction = () =>
                {
                };
                onFinishedCasting = () =>
                {
                };
            }

            onFinishedCasting += () => onFinishedCasting = () => { };
        }

        public abstract void PlaceAction(GameAction action, GameCoordinate placeLocation);
        public abstract void Dispose();

        public void ClearSelected()
        {
            SelectedAction = Owner.DefaultAction;
            CurrentButtonSelected = null;
            Player.Cursor.SetIcon(Owner.DefaultAction.Icon);
        }
    }

    public class NPCCombatActionHandler : CombatActionHandler
    {
        public NPCCombatActionHandler(IActionCapable owner) : base(owner)
        {

        }

        public void TryPlaceAction(GameAction action, NPCState state)
        {
            var Location = action.NPCActionPlacementCalculator(state);
            if (Location == null) Location = Owner.Location;
            action.ForcePlaced = true;
            action.PlacedLocation = Location;
            PlaceAction(action, action.PlacedLocation);
        }
    }

    public class CombatActionHandler : ActionHandler
    {
        private SubsequentlyPlacedActions SubsequentlyPlacedActions;

        public CombatActionHandler(IActionCapable owner) : base(owner)
        {
            SubsequentlyPlacedActions = new SubsequentlyPlacedActions();
        }

        public override void PlaceAction(GameAction action, GameCoordinate placeLocation)
        {
            if (SelectedAction == null) return;
            SelectedAction.PayPreConditions();
            SubsequentlyPlacedActions.Add(action);
            SelectedAction.Place(placeLocation, SelectedAction.Icon);
        }

        public override ActionStatus CommitActions(object args)
        {
            if (SubsequentlyPlacedActions.Count() == 0) return ActionStatus.NoAction;
            var first = SubsequentlyPlacedActions.First();
            var finished = first.GetAction().Invoke(args);
            if (finished)
            {
                onFinishedCasting.Invoke();
                SubsequentlyPlacedActions.RemoveFirst();
                if (SubsequentlyPlacedActions.Count() == 0) return ActionStatus.AllFinished;
                return ActionStatus.Finished;
            }
            else if (first.IsPlaced == false) return ActionStatus.Placing;

            return ActionStatus.Ongoing;
        }

        public override void OnMouseDown(GameCoordinate mouseLocation)
        {
            if (SelectedAction == null) return;
            if (Owner.ActionStatus == ActionStatus.WaitingForOther) return;
            if (!SelectedAction.PlacementFilter(mouseLocation)) return;

            if (SubsequentlyPlacedActions.AlreadyPlaced(SelectedAction))
            {
                SubsequentlyPlacedActions.Remove(SelectedAction);
            }

            //Update location of rangeshape and line.
            //line's location:
            //  origin: last placed move location or hero's location if none placed.
            //  terminus: mouse click location
            //Rangeshape's location: last placed move location or hero's location if none placed.
            var lma = SubsequentlyPlacedActions.LastMoveAction();
            
            Entity newFollow;
            if (lma != null)
            {
                newFollow = lma.Marker;
            }
            else
            {
                newFollow = Owner as Unit;
            }
            SelectedAction.RangeShape.Following = newFollow;
            SelectedAction.ActionLine.Set(newFollow, mouseLocation);

            SelectedAction.RangeShape.Visible = true;
        }

        public override void Dispose()
        {
            SubsequentlyPlacedActions.Clear();
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

        public bool TryRemoveIdenticalType(GameAction ga)
        {
            if (this.Contains(ga))
            {
                this.RemoveGameAction(ga);
                return true;
            }

            return false;
        }
    }

    public class OutOfCombatActionHandler : ActionHandler
    {
        OutOfCombatPlacedActions PlacedActions = new OutOfCombatPlacedActions();

        public OutOfCombatActionHandler(IActionCapable owner) : base(owner)
        {

        }

        public override void PlaceAction(GameAction action, GameCoordinate placeLocation)
        {
            var removed = PlacedActions.TryRemoveIdenticalType(action);

            if(Owner.ActionStatus != ActionStatus.Ongoing || removed)
            { 
                SelectedAction.Place(placeLocation, SelectedAction.Icon);
                if (SelectedAction.IsPlaced)
                    PlacedActions.Add(action);
            }
        }

        public override void OnMouseDown(GameCoordinate mouseLocation)
        {
            if (SelectedAction == null) return;
            SelectedAction.RangeShape.Visible = true;
        }

        public override void Dispose()
        {
            //remove all
            PlacedActions.RemoveWhere(e => true);
        }

        public override ActionStatus CommitActions(object args)
        {
            if (PlacedActions.Count == 0) return ActionStatus.NoAction;

            var currentAction = PlacedActions.First();

            var finished = currentAction.GetAction().Invoke(args);
            if (finished)
            {
                onFinishedCasting.Invoke();
                PlacedActions.RemoveGameAction(currentAction);
                if (PlacedActions.Count == 0) return ActionStatus.AllFinished;
                return ActionStatus.Finished;
            }

            return ActionStatus.Ongoing;

        }
    }
}
