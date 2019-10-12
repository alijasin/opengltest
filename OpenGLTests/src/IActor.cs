using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src
{
    public interface IActor
    {
        CombatActionHandler CombatActionHandler { get; set; }
        GameCoordinate Location { get; set; }
        //void CommitAction(Action Action);
        //void DoCommitedActions();
    }
}
