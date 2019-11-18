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

            //calc movement vector and do the usual stuff
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

            // if (blockedX && blockedY || blockedX && Math.Abs(dy) < speed.Y || blockedY && Math.Abs(dx) < speed.X) return true; //we are kinda stuck
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


            bool blockedX = false;
            bool blockedY = false;
            if (collisionCheck)
            {
                //:: Optimizable Area
                // this should be done after/before game logic is made. So we dont need to iterate all entities everytime an entity moves, but instead
                // only do it at one place once.
                foreach (var collidable in GameState.Drawables.GetAllCollidables.Where(e => !e.Phased && e != source))
                {
                    if (collisionCheckFilter.Any(t => collidable.GetType().IsSubclassOf(t) || t == collidable.GetType())) continue;

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
            }

            if (!blockedY) source.Location.Y -= velY;
            if (!blockedX) source.Location.X -= velX;
            if (blockedX && blockedY || blockedX && Math.Abs(dy) < speed.Y || blockedY && Math.Abs(dx) < speed.X) return true; //we are kinda stuck
            return false;
        }
    }
}
