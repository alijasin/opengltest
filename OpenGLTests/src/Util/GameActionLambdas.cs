using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    public static class GameActionLambdas
    {
        public static ICombatable FindCombatableLambda(RangeShape lookingShape, ICombatable source)
        {
            foreach (var others in GameState.Drawables.GetAllCombatables.Where(d => d != source))
            {
                if (lookingShape.Contains(others.Location))
                {
                    return others;
                }
            }

            return null;
        }

        public static bool MoveTowardsPoint(ICombatable source, GameCoordinate point)
        {
            if (source.Location.Distance(point) < source.Speed.X || source.Location.Distance(point) < source.Speed.Y)
            {
                //we are close enough
                return true;
            }
            else
            {
                var dx = point.X - source.Location.X;
                var dy = point.Y - source.Location.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                var velX = (dx / dist) * source.Speed.X;
                var velY = (dy / dist) * source.Speed.Y;

                source.Location.X += (float)velX;
                source.Location.Y += (float)velY;
                return false;
            }
        }


        public static bool MoveAwayFromPoint(ICombatable source, GameCoordinate point)
        {
            if (source.Location.Distance(point) < source.Speed.X || source.Location.Distance(point) < source.Speed.Y)
            {
                return true;
            }
            else
            {
                var dx = point.X - source.Location.X;
                var dy = point.Y - source.Location.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                var velX = (dx / dist) * source.Speed.X;
                var velY = (dy / dist) * source.Speed.Y;

                source.Location.X -= (float)velX;
                source.Location.Y -= (float)velY;
                return false;
            }
        }
    }
}
