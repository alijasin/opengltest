using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;
using OpenGLTests.src.Util;

namespace OpenGLTests
{
    class Program
    {
        static void Main(string[] args)
        {
            GraphicsController.Init();
            //InputController.Init();
            GameController gc = new GameController();
            gc.StartGame();
        }
    }
}
