using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    class RoomLoader
    {
        public enum Room
        {
            TestSpace,
            Overworld,
            RandomGenerated
            
        }
        public static void LoadRoom(Room room)
        {
            if (room == Room.RandomGenerated)
            {
                Button b = new Button(new GLCoordinate(0.2f, 0.2f));
                b.OnInteraction = () =>
                {
                    RoomGenerator.Step();
                };
                b.Location = new GLCoordinate(-1, 0);

                RoomGenerator.RecursiveBacktrackingMaze();
                b.Color = Color.Green;
                GameState.Drawables.Add(b);
            }
            else EntitySerializer.LoadEntitiesFromFile(room.ToString());
        }
    }
}
