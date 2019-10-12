using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    class ActionPattern
    {
        public List<GameAction> Actions;
    }

    class MoveAroundAndChill : ActionPattern
    {
        public MoveAroundAndChill(Entity source) 
        {
            Actions = new List<GameAction>()
            {
                new MoveTowardsAction(RNG.RandomPointWithinCircle(new GLCoordinate(0.4f, 0.4f)), source),
                new ChillAction(),
            };
        }
    }
}
