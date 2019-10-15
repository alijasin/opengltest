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

    }
}
