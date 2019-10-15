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
    public class MarkerLine : Drawable
    {
        private Line line;
        private GameCoordinate terminus;
        private GameCoordinate origin;

        public MarkerLine(LinkedList<GameAction> actions, IActionCapable owner)
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

        public void Add(GameAction placed, IActionCapable owner)
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
        private Button CurrentActionButton { get; set; }
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

            //Update current action
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
        }

        /// <summary>
        /// Called from Screen when mouse has been released.
        /// </summary>
        /// <param name="location"></param>
        public void Up(GameCoordinate location)
        {
            if (CurrentAction == null) return;
            CurrentAction.RangeShape.Visible = false;

            if (CurrentAction.RangeShape.Contains(location))
            {
                if (CurrentAction.Ready)
                {

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

    }

    public class OutOfCombatActionHandler
    {

    }
}
