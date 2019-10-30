using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    public static class ContainsLambdas
    {
        public static bool RectangleContains(Element d, GameCoordinate point)
        {
            var x = Math.Abs(point.X - d.Location.X);
            var y = Math.Abs(point.Y - d.Location.Y);
            //todo move this to somewhere else and fuck yourself.
            GLCoordinate clicked = new GLCoordinate(x * 2 / GibbWindow.WIDTH - 1, -(y * 2 / GibbWindow.HEIGHT - 1));

            return d.Location.X - d.Size.X / 2 < clicked.X && d.Location.X + d.Size.X / 2 > clicked.X &&
                   d.Location.Y - d.Size.Y / 2 < clicked.Y && d.Location.Y + d.Size.Y / 2 > clicked.Y;
        }

        public static bool RectangleContains(Entity d, GameCoordinate point)
        {
            var x = Math.Abs(point.X - d.Location.X);
            var y = Math.Abs(point.Y - d.Location.Y);
            //todo move this to somewhere else and fuck yourself.
            GLCoordinate clicked = new GLCoordinate(x * 2 / GibbWindow.WIDTH - 1, -(y * 2 / GibbWindow.HEIGHT - 1));

            return d.Location.X - d.Size.X / 2 < clicked.X && d.Location.X + d.Size.X / 2 > clicked.X &&
                   d.Location.Y - d.Size.Y / 2 < clicked.Y && d.Location.Y + d.Size.Y / 2 > clicked.Y;
        }

    }
}
