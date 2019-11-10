using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    public static class MyMath
    {
        public static int AngleBetweenTwoPoints(GameCoordinate p1, GameCoordinate p2)
        {
            if (p1 == null || p2 == null) return 0;
            var deltaX = p1.X - p2.X;
            var deltaY = p1.Y - p2.Y;
            var alpha = Math.Atan2(deltaY, -deltaX);
            int alphaDeg = (int)((alpha * 180 / Math.PI + 360) % 360);
            return alphaDeg;
        }

        public static float DistanceBetweenTwoPoints(GameCoordinate p1, GameCoordinate p2)
        {
            if (p1 == null || p2 == null) return 0;
            var deltaX = p1.X - p2.X;
            var deltaY = p1.Y - p2.Y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
