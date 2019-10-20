using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    public class MazeCreator
    {
        private GameCoordinate up;
        private GameCoordinate down;
        private GameCoordinate left;
        private GameCoordinate right;
        private Type fabricType;
        private GameCoordinate origin;
        private GameCoordinate terminus;

        public MazeCreator(Entity mazeFabric, GameCoordinate origin, GameCoordinate terminus)
        {
            mazeFabric.Size = new GLCoordinate(mazeFabric.Size.X, mazeFabric.Size.Y);
            up = new GameCoordinate(0, mazeFabric.Size.Y);
            down = new GameCoordinate(0, -mazeFabric.Size.Y);
            left = new GameCoordinate(-mazeFabric.Size.X, 0);
            right = new GameCoordinate(mazeFabric.Size.X, 0);
            fabricType = mazeFabric.GetType();

            this.origin = origin;
            this.terminus = terminus;
        }

        public void CreateMaze()
        {
            GameCoordinate current = new GameCoordinate(0, 0);

            while(current.X > origin.X && current.Y > origin.Y && current.Y < terminus.Y && current.X < terminus.X)
            {
                GameCoordinate dir = direction();
                current += dir;
            }
        }

        private GameCoordinate direction()
        {
            var r = RNG.IntegerBetween(0, 4);

            switch (r)
            {
                case 0: return up;
                case 1: return down;
                case 2: return left;
                case 3: return right;
            }
            return new GameCoordinate(0, 0);
        }
    }
}
