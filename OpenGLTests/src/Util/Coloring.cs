using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Util
{
    static class Coloring
    {
        public static Color Opposite(Color c)
        {
            return Color.FromArgb(c.ToArgb() ^ 0xffffff);
        }

        public static Color FromRarity(Rarity Rarity)
        {
            switch (Rarity)
            {
                case Rarity.Common: return Color.Gray;
                case Rarity.Uncommon: return Color.GreenYellow;;
                case Rarity.Rare: return Color.DarkBlue;
                case Rarity.Epic: return Color.Purple;
                case Rarity.Legendary: return Color.DarkOrange;
                default: return Color.Cornsilk;
            }
        }
    }
}
