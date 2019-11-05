using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Terrain;

namespace OpenGLTests.src.Drawables
{
    public interface IRegion
    {
        RangeShape Shape { get; set; }
        void OnEntered(Unit d);
    }

    class RoomLoadRegion : Entity, IRegion
    {
        private RoomLoader.Room room;
        public RoomLoadRegion(GameCoordinate loc, RoomLoader.Room room)
        {
            this.Location = loc;
            this.room = room;
            this.Visible = false;
            Shape = new RangeShape(new Rectangle(new GLCoordinate(0.4f, 0.2f)), this);
            Shape.Visible = true;

            EnteredFilter = drawable => drawable is Hero;
        }

        public RangeShape Shape { get; set; }
        public Func<Drawable, bool> EnteredFilter { get; set; }

        public void OnEntered(Unit d)
        {
            if (EnteredFilter(d))
            {
                Console.WriteLine("loading " + room);
                //todo: this doesnt work. Or I mean. It works but I dont want it this way.
                GameState.Drawables.ClearExcept(drawable => drawable is Hero || drawable is Floor || drawable is RangeShape || drawable is Marker || drawable is Line || drawable is Button);
                RoomLoader.LoadRoom(room);
                EnteredFilter = movable => false; //only enter one time.
            }
        }
    }
}
