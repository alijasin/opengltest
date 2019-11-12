using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    class LootEntry
    {
        public int Probability { get; set; }
        public Item Item { get; set; }

        public LootEntry(Item i, int prob)
        {
            this.Probability = prob;
            this.Item = i;
        }
    }

    class LootTable
    {
        private List<LootEntry> lootEntries = new List<LootEntry>();

        public LootTable(params LootEntry[] lootEntries)
        {
            foreach(var le in lootEntries) this.lootEntries.Add(le);
        }

        public List<Item> DropLoot()
        {
            List<Item> toDrop = new List<Item>();
            var rng = RNG.IntegerBetween(0, 101);
            foreach (var loot in lootEntries)
            {
                if (loot.Probability <= rng)
                {
                    toDrop.Add(loot.Item);
                }
            }

            return toDrop;
        }
    }
}
