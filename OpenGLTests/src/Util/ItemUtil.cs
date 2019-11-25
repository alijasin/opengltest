using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Entities.Equipment;

namespace OpenGLTests.src.Util
{
    public static class ItemUtil
    {
        /// <summary>
        /// Send in item with owner = new Dummy()
        /// </summary>
        /// <param name="i"></param>
        /// <param name="loc"></param>
        public static void SpawnItem(Item i, GameCoordinate loc)
        {
            new DroppedItem<Item>(i, loc);
        }
    }
}
