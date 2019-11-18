using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    //not so much lambdas lmao
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

        public static bool MoveTowardsPoint(Unit source, GameCoordinate point, params Type[] collisionCheckFilter)
        {
            return MoveTowardsPoint(source, point, source.Speed, true, collisionCheckFilter);
        }

        public static bool FlyTowardsPoint(Unit source, GameCoordinate point)
        {
            return MoveTowardsPoint(source, point, source.Speed, false);
        }

        public static bool FlyTowardsPoint(Unit source, GameCoordinate point, GameCoordinate speed)
        {
            return MoveTowardsPoint(source, point, speed, false);
        }

        public static bool MoveTowardsPoint(Unit source, GameCoordinate point, GameCoordinate speed, bool collisionCheck = true, params Type [] collisionCheckFilter)
        {
            if (source.Location.Distance(point) < speed.X || source.Location.Distance(point) < speed.Y) return true; //if close enough to target

            //todo: consider whether this should be here.
            if (source.Location.X < point.X)
            {
                source.SetFacing(Facing.Right);
            }
            else
            {
                source.SetFacing(Facing.Left);
            }

            //calc movement vector 
            var dx = point.X - source.Location.X;
            var dy = point.Y - source.Location.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            var velX = (float)(dx / dist) * speed.X;
            var velY = (float)(dy / dist) * speed.Y;


            if (velX > 0 && !source.BlockedSides.BlockedRight || velX < 0 && !source.BlockedSides.BlockedLeft)
            {
                source.Location.X += velX;
            }

            if (velY > 0 && !source.BlockedSides.BlockedTop || velY < 0 && !source.BlockedSides.BlockedBottom)
            {
                source.Location.Y += velY;
            }


            if (Math.Abs(dy) < speed.Y || Math.Abs(dx) < speed.X) return true; //we are kinda stuck
            return false;
        }
       
        public static bool MoveAwayFromPoint(Unit source, GameCoordinate point, bool collisionCheck = true, params Type[] collisionCheckFilter)
        {
            var speed = source.Speed;
            if (source.Location.Distance(point) < speed.X || source.Location.Distance(point) < speed.Y) return true; //if close enough to target

            //todo: consider whether this should be here.
            if (source.Location.X < point.X)
            {
                source.SetFacing(Facing.Right);
            }
            else
            {
                source.SetFacing(Facing.Left);
            }

            //calc movement vector and do the usual stuff
            var dx = point.X - source.Location.X;
            var dy = point.Y - source.Location.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            var velX = (float)(dx / dist) * speed.X;
            var velY = (float)(dy / dist) * speed.Y;


            if (velX > 0 && !source.BlockedSides.BlockedLeft || velX < 0 && !source.BlockedSides.BlockedRight)
            {
                source.Location.X -= velX;
            }
            if (velY > 0 && !source.BlockedSides.BlockedBottom || velY < 0 && !source.BlockedSides.BlockedTop)
            {
                source.Location.Y -= velY;
            }

            //if (blockedX && blockedY || blockedX && Math.Abs(dy) < speed.Y || blockedY && Math.Abs(dx) < speed.X) return true; //we are kinda stuck
            return false;
        }
    }
}
