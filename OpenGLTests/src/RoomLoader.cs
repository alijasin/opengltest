using System;
using System.Collections.Generic;
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
            if (room == Room.RandomGenerated) RoomGenerator.RecursiveBacktrackingMaze();
            else EntitySerializer.LoadEntitiesFromFile(room.ToString());
        }
    }
}
