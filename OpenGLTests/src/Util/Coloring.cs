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
    }
}
