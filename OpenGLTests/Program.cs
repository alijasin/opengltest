using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;
using OpenGLTests.src.Util;

namespace OpenGLTests
{
    class Program
    {
        public static bool EDITOR = false;
        static void Main(string[] args)
        {
            GraphicsController.Init();
            //InputController.ReInit();

            if (EDITOR == false)
            {
                GameController gc = new GameController();
                gc.StartGame();
            }
            else
            {
                EditorController ec = new EditorController();
                ec.Init();
            }

        }
    }
}
