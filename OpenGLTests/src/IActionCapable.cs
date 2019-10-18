using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src
{
    public interface IActionCapable
    {
        IActionHandler ActionHandler { get; set; }
        GameCoordinate Location { get; set; }
        bool InCombat { get; set; }
    }
}
