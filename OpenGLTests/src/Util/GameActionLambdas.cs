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
        public static Unit FindUnitLambda(RangeShape lookingShape, Unit source)
        {
            foreach (var others in GameState.Drawables.GetAllUnits.Where(d => d != source))
            {
                if (lookingShape.Contains(others.Location))
                {
                    return others;
                }
            }

            return null;
        }

        public static Hero FindHeroLambda(RangeShape lookingShape)
        {
            foreach (var others in GameState.Drawables.GetAllHeroes)
            {
                if (lookingShape.Contains(others.Location))
                {
                    return others;
                }
            }

            return null;
        }
        public static bool MoveTowardsPoint(Unit source, GameCoordinate point)
        {
            if (source.Location.Distance(point) < source.Speed.X || source.Location.Distance(point) < source.Speed.Y)
            {
                //we are close enough
                return true;
            }
            if (source.Location.X < point.X)
            {
                source.Facing = Facing.Right;
            }
            else
            {
                source.Facing = Facing.Left;
            }


            var dx = point.X - source.Location.X;
            var dy = point.Y - source.Location.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            var velX = (float)(dx / dist) * source.Speed.X;
            var velY = (float)(dy / dist) * source.Speed.Y;


            bool blockedX = false;
            bool blockedY = false;

            //:: Optimizable Area
            //1. filter these collidables some more so we dont iterate all of them.
            //2. store some things in variables
            foreach (var collidable in GameState.Drawables.GetAllCollidables.Where(e => !e.Phased && e != source))
            {
                if (collidable.BoundingBox.Contains(new GameCoordinate(collidable.BoundingBox.Location.X,
                        source.Location.Y + velY * 2))
                    && collidable.BoundingBox.Contains(new GameCoordinate(source.Location.X + velX / 2,
                        collidable.BoundingBox.Location.Y)))
                {
                    blockedY = true;
                }

                if (collidable.BoundingBox.Contains(new GameCoordinate(source.Location.X + velX * 2,
                        collidable.BoundingBox.Location.Y))
                    && collidable.BoundingBox.Contains(new GameCoordinate(collidable.BoundingBox.Location.X,
                        source.Location.Y + velY / 2)))
                {
                    blockedX = true;
                }
            }

            if (!blockedY) source.Location.Y += velY;
            if (!blockedX) source.Location.X += velX;

            return false;
        }


        public static bool MoveAwayFromPoint(Unit source, GameCoordinate point)
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
