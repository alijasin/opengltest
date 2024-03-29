﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    static class RNG
    {
        static Random r = new Random(Guid.NewGuid().GetHashCode());

        //usage: on step: location += RNG.RandomMovementDelta()
        //recommendation: no usage.
        public static GameCoordinate RandomMovementDelta()
        {
            var n = r.Next() % 25;

            switch (n)
            {
                case 1: return new GameCoordinate(0.01f, 0);
                case 2: return new GameCoordinate(0, 0.01f);
                case 3: return new GameCoordinate(0.01f, 0.01f);
                case 4: return new GameCoordinate(-0.01f, 0);
                case 5: return new GameCoordinate(0, -0.01f);
                case 6: return new GameCoordinate(-0.01f, -0.01f);
                default: return new GameCoordinate(0, 0);
            }
        }

        /// <summary>
        /// Return a random point within radius.
        /// </summary>
        /// <param name="Radius"></param>
        /// <returns></returns>
        public static GameCoordinate RandomPointWithinCircle(GLCoordinate Radius)
        {
            var re = Radius.X * Math.Sqrt(r.NextDouble());
            var theta = r.NextDouble() * 2 * (float)Math.PI;

            return new GameCoordinate((float) (re * (float)Math.Cos(theta)), (float) (re * (float)Math.Sin(theta)));
        }

        public static GameCoordinate RandomPointWithinCircleRelativeToLocation(GameCoordinate location, GLCoordinate circleRadius)
        {
            var pointInCircle = RandomPointWithinCircle(circleRadius);
            return new GameCoordinate(location.X + pointInCircle.X, location.Y + pointInCircle.Y);
        }

        /// <summary>
        /// Think this should return true if x out of y times.
        /// For example x = 5, y = 100. This would return true 5 in 100 times.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool XTimesInY(int x, int y)
        {
            return r.Next() % y < x;
        }

        /// <summary>
        /// Returns an integer between a(including) and b(excluding)
        /// </summary>
        /// <param name="a">min</param>
        /// <param name="b">max</param>
        /// <returns> a <= n < b </returns>
        public static int IntegerBetween(int a, int b)
        {
            return r.Next(a, b);
        }

        public static float BetweenZeroAndOne()
        {
            return (float)r.NextDouble();
        }

        public static int NegativeOrPositiveOne()
        {
            return r.Next() % 2 == 0 ? 1 : -1;
        }

        public static Color RandomColor()
        {
            return Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255));
        }

        public static GameCoordinate RandomLocationWithinSquare(GameCoordinate origin, GameCoordinate terminus)
        {
            //assert origin < terminus bla bla..
            var rngLoc = new GameCoordinate((float)r.NextDouble() % (terminus.X - origin.X) + origin.X, (float)r.NextDouble() % (terminus.Y - origin.Y) + origin.Y);
            return rngLoc;
        }

        public static T RandomElement<T>(List<T> list)
        {
            return list.ElementAt(RNG.IntegerBetween(0, list.Count()));
        }
    }
}
