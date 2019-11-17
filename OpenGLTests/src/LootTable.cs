using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public class LootEntry
    {
        public int Probability { get; set; }
        public Item Item { get; set; }

        public LootEntry(Item i, int prob)
        {
            this.Probability = prob;
            this.Item = i;
        }
    }

    public class LootTable
    {
        private List<LootEntry> lootEntries;

        public LootTable(params LootEntry[] lootEntries)
        {
            this.lootEntries = new List<LootEntry>();
            if (lootEntries == null) return;
            foreach (var le in lootEntries) this.lootEntries.Add(le);
        }

        public void DropLoot(GameCoordinate loc)
        {
            var rng = RNG.IntegerBetween(0, 101);
            foreach (var loot in lootEntries)
            {
                if (loot.Probability > rng)
                {
                    new DroppedItem(loot.Item, RNG.RandomPointWithinCircleRelativeToLocation(loc, new GLCoordinate(0.2f, 0.2f)));
                }
            }
        }
    }
}
