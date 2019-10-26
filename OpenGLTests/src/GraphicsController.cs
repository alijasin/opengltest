using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;
using OpenTK;

namespace OpenGLTests
{
    static class GraphicsController
    {
        public static GibbWindow Window { get; set; }
        public static Screen ActiveScreen { get; set; }
        private static ManualResetEventSlim mre;

        public static void Init()
        {
            if (Window != null)
            {
                Console.WriteLine("Tried to initialize GraphicsController when it has already been initialized.");
                return;
            }

            Thread t = new Thread(xd);
            t.Start();
            while (mre == null)
            {
                Thread.Sleep(1);
            }
            mre.Wait();
        }

        private static void xd()
        {
            Window = new GibbWindow(Render);
            Window.Visible = false;
            mre = new ManualResetEventSlim();
            Window.Load += (_, __) => mre.Set();
            Window.Run();
        }

        private static void Render(DrawAdapter drawAdapter)
        {
            if (ActiveScreen != null)
            {
                ActiveScreen.Draw(drawAdapter);
            }
        }

    }
}
