using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Util
{
    static class IDGenerator
    {
        private static int id = 0;
        public static int GetID()
        {
            return ++id;
        }
    }
}
