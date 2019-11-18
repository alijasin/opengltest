using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Terrain;

using OpenGLTests.src.Util;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class GameState
    {
        public static List<Player> Players { get; set; }
        public Hero Hero { get; set; }  //todo, move this to Player
        public static DrawableRepository Drawables = new DrawableRepository();
        public static List<StatusEffect> StatusEffects = new List<StatusEffect>();
        public static RainGenerator RainGenerator = new RainGenerator(RainGenerator.RainType.Clear); //todo: move to drawable

        public GameState()
        {
            Players = new List<Player>();
            Hero = new Hero();
            Hero.Location = new GameCoordinate(0, 0);
            Players.Add(new Player(Hero));
            Drawables.Add(Hero);
            /*Drawables.Add(new RoomLoadRegion(new GameCoordinate(0.8f, 0.8f), RoomLoader.Room.TestSpace));
            Drawables.Add(new FanBoy(new GameCoordinate(0.7f, -0.5f)));
            Drawables.Add(new Campfire(new GameCoordinate(0, 0.3f)));
            Drawables.Add(new BearTrap(new GameCoordinate(0, 0.2f)));
            new Wizard(new GameCoordinate(-0.5f, 0));*/
            RoomLoader.LoadRoom(RoomLoader.Room.TestEditorOutPut);
        }


        private void LoadTestRoom()
        {
            RoomLoader rl = new RoomLoader();
        }

        private Fight fight = new Fight();
        private int initSteps = 25;
        private int initStepsCount = 0;

        public void Step()
        {
            foreach(var playa in Players) Camera.ActiveCamera.Step();

            if (initStepsCount < initSteps)
            {
                initStepsCount++;
                return;
            }

            foreach (var effect in StatusEffects)
            {
                effect.TryApplyEffect();
            }

            for (int i = StatusEffects.Count - 1; i >= 0; i--)
            {
                if(StatusEffects.ElementAt(i).LiveTime == 0) StatusEffects.RemoveAt(i);
            }


            //:: Optimization point
            //this shuold only be movable units
            foreach (var reg in Drawables.GetAllRegions)
            {
                foreach (Unit e in Drawables.GetAllUnits)
                {
                    if (reg.Shape.Contains(e.Location))
                    {
                        reg.OnEntered(e);
                    }
                }
            }

            if (!fight.LastManStanding())
            {
                var currentFighter = fight.GetCurrentTurn();

                foreach (var h in fight.Heroes())
                {
                    if(h == currentFighter) h.Player.SetCameraToDefault();
                    else h.Player.SetFightCamera(fight);
                }

                if (currentFighter != null) //redundant?
                {
                    currentFighter.CombatStep(fight);
                }
            }

            foreach (var unit in Drawables.GetAllUnits)
            {
                if (unit.InCombat == false)
                {
                    unit.OutOfCombatStep();
                }
            }

            foreach (var effect in Drawables.GetAllEffects)
            {
                effect.LogicStep();
            }

            foreach (Unit aggro in Drawables.GetAllUnits
                .Where(c => !(c is Hero) && c.AggroShape != null && c.InCombat == false).ToList())
            {
                if (aggro.AggroShape.Contains(Hero.Location))
                {
                    aggro.OnAggro(Hero);
                    Hero.OnAggro(aggro);
                    fight.AddFighter(Hero);
                    fight.AddFighter(aggro);
                }
            }

            //todo: we are assuming all bounding boxes are rectangles. this might not be acceptable later on.
            foreach (ICollidable collided in Drawables.GetAllCollidables)
            {
                BlockedSides bs = new BlockedSides();
                foreach (ICollidable collider in Drawables.GetAllCollidables)
                {
                    if (collider == collided) continue;
                    if (collider.Phased) continue;

                    var rectA = collider.BoundingBox;
                    var rectB = collided.BoundingBox;

                    float player_bottom = collider.BoundingBox.Bottom;
                    float tiles_bottom = collided.BoundingBox.Bottom;
                    float player_right = collider.BoundingBox.Right;
                    float tiles_right = collided.BoundingBox.Right;

                    float b_collision = tiles_bottom - collider.BoundingBox.Top;
                    float t_collision = player_bottom - collided.BoundingBox.Top;
                    float l_collision = player_right - collided.BoundingBox.Left;
                    float r_collision = tiles_right - collider.BoundingBox.Left;
                    
                    //if rectangle intersection
                    if (rectA.Left < rectB.Right && rectA.Right > rectB.Left && rectA.Top > rectB.Bottom && rectA.Bottom < rectB.Top)
                    {
                        if (rectA.Left < rectB.Right && rectA.Right > rectB.Right && r_collision > b_collision) bs.BlockedRight = true;
                        if (rectA.Right > rectB.Left && rectA.Left < rectB.Left && l_collision > b_collision) bs.BlockedLeft = true;
                        if (rectA.Bottom < rectB.Top && rectA.Top < rectB.Top && t_collision < r_collision) bs.BlockedBottom = true;
                        if (rectA.Top > rectB.Bottom && rectA.Bottom > rectB.Bottom && b_collision < r_collision) bs.BlockedTop = true;
                    }

                    if (bs.BlockedLeft && bs.BlockedRight && (bs.BlockedTop || bs.BlockedBottom))
                    {
                        bs.BlockedLeft = bs.BlockedRight = false;
                    }

                    if (bs.BlockedTop && bs.BlockedBottom && (bs.BlockedLeft || bs.BlockedRight))
                    {
                        bs.BlockedTop = bs.BlockedBottom = false;
                    }
                }
                collided.BlockedSides = bs;
            }

        }
    }
}
