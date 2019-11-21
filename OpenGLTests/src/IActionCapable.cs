using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src
{
    public interface IActionCapable
    {
        ActionHandler ActionHandler { get; set; }
        GameAction DefaultAction { get; set; }
        GameCoordinate Location { get; set; }
    }
}
