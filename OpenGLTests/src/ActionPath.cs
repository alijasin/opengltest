﻿using System;
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
        protected SpriteID SelectedActionIcon { get; set; }
        public IActionCapable Owner { get; set; }
        public GameAction SelectedAction { get; set; }

        public ActionHandler(IActionCapable owner)
        {
            Owner = owner;
        }

        public abstract void TryPlaceAction(GameAction action, GameCoordinate location);

        public abstract ActionReturns CommitActions(object args);

        public void OnMouseUp(GameCoordinate mouseLocation)
        {
            if (SelectedAction == null) return;

            TryPlaceAction(SelectedAction, mouseLocation);
            SelectedAction.RangeShape.Visible = false;
        }

        public abstract void OnMouseDown(GameCoordinate mouseLocation);

        public void ActionButtonClicked(ActionButton actionButton)
        {
            SelectedAction = actionButton.GameAction;
            SelectedActionIcon = actionButton.Animation.GetSprite().sid;
        }

        public abstract void Dispose();
    }

    public class CombatActionHandler : ActionHandler
    {
        private SubsequentlyPlacedActions SubsequentlyPlacedActions;

        public CombatActionHandler(IActionCapable owner) : base(owner)
        {
            SubsequentlyPlacedActions = new SubsequentlyPlacedActions();
        }

        public override void TryPlaceAction(GameAction action, GameCoordinate location)
        {
            if (action.RangeShape.Contains(location) || action.RangeShape.IsInfinite || action.IsInstant || action.ForcePlaced)
            {
                SubsequentlyPlacedActions.Add(action);
                action.Place(location, SelectedActionIcon);
            }
        }

        public override ActionReturns CommitActions(object args)
        {
            if (SubsequentlyPlacedActions.Count() == 0) return ActionReturns.NoAction;
            var first = SubsequentlyPlacedActions.First();
            var finished = first.GetAction().Invoke(args);
            if (finished)
            {
                SubsequentlyPlacedActions.RemoveFirst();
                if (SubsequentlyPlacedActions.Count() == 0) return ActionReturns.AllFinished;
                return ActionReturns.Finished;
            }
            else if (first.IsPlaced == false) return ActionReturns.Placing;

            return ActionReturns.Ongoing;
        }

        public override void OnMouseDown(GameCoordinate mouseLocation)
        {
            if (SelectedAction == null) return;

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
    }

    public class OutOfCombatActionHandler : ActionHandler
    {
        OutOfCombatPlacedActions PlacedActions = new OutOfCombatPlacedActions();

        public OutOfCombatActionHandler(IActionCapable owner) : base(owner)
        {

        }

        public override void TryPlaceAction(GameAction action, GameCoordinate location)
        {
            //If clicked within range or if the range is infinite
            if (action.IsInstant)
            {
                PlacedActions.Add(action);
            }
            else if (action.RangeShape.IsInfinite || action.RangeShape.Contains(location))
            {
                //remove all placed actions that are identical to the new one.
                PlacedActions.RemoveWhere(pa => pa.GetType() == action.GetType());

                action.Place(location, SelectedActionIcon);
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

        public override ActionReturns CommitActions(object args)
        {
            if (PlacedActions.Count == 0) return ActionReturns.NoAction;

            var currentAction = PlacedActions.First();

            var finished = currentAction.GetAction().Invoke(args);
            if (finished)
            {
                PlacedActions.RemoveGameAction(currentAction);
                if (PlacedActions.Count == 0) return ActionReturns.AllFinished;
                return ActionReturns.Finished;
            }

            return ActionReturns.Ongoing;

        }
    }
}
