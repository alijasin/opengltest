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
        GameCoordinate Location { get; set; }
        bool CombatStep(int combatIndex);

    }
}
