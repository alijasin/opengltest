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
            g1 = new Apple(GameState.Drawables.GetAllHeroes.First());
            g2 = new Apple(GameState.Drawables.GetAllHeroes.First());
            g3 = new Apple(GameState.Drawables.GetAllHeroes.First());
            g4 = new Apple(GameState.Drawables.GetAllHeroes.First());
            g2.Size = new GLCoordinate(0.01f, 0.01f);
            g3.Size = new GLCoordinate(0.01f, 0.01f);
            g4.Size = new GLCoordinate(0.01f, 0.01f);
            g1.Size = new GLCoordinate(0.01f, 0.01f);
            GameState.Drawables.Add( g1);
            GameState.Drawables.Add( g2);
            GameState.Drawables.Add( g3);
            GameState.Drawables.Add(g4);

        }

        private static Apple g1;
        private static Apple g2;
        private static Apple g3;
        private static Apple g4;
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

            //todo: if needed the algo can be improved.
            //todo: significant improvement would be that we only check collided if they have movement speed. <-- this is needed not recommended later on.
            //todo: we are assuming all bounding boxes are rectangles. this might not be acceptable later on.
            foreach (ICollidable collider in Drawables.GetAllCollidables)
            {
                BlockedSides bs = new BlockedSides();
                foreach (ICollidable collided in Drawables.GetAllCollidables)
                {
                    if (collided == collider) continue;
                    if (collided.Phased) continue;
                    if (bs.BlockedCount() >= 2) continue; 


                    var rectA = collided.BoundingBox;
                    var rectB = collider.BoundingBox;

                    if(rectA.Contains(new GameCoordinate(rectB.XCenter, rectB.Top)))
                    {
                        bs.BlockedTop = true;
                    }
                    if (rectA.Contains(new GameCoordinate(rectB.XCenter, rectB.Bottom)))
                    {
                        bs.BlockedBottom = true;
                    }
                    if (rectA.Contains(new GameCoordinate(rectB.Left, rectB.YCenter)))
                    {
                        bs.BlockedLeft = true;
                    }
                    if (rectA.Contains(new GameCoordinate(rectB.Right, rectB.YCenter)))
                    {
                        bs.BlockedRight = true;
                    }
                    if (rectA.Contains(new GameCoordinate(rectB.Left + rectB.Size.X / 4, rectB.Top - rectB.Size.Y / 4)))
                    {
                        bs.BlockedTop = bs.BlockedLeft = true;
                    }
                    else if(rectA.Contains(new GameCoordinate(rectB.Left + rectB.Size.X / 4, rectB.Bottom + rectB.Size.Y / 4)))
                    {
                        bs.BlockedBottom = bs.BlockedLeft = true;
                    }
                    else if (rectA.Contains(new GameCoordinate(rectB.Left + rectB.Size.X / 4, rectB.Bottom + rectB.Size.Y / 4)))
                    {
                        bs.BlockedTop = bs.BlockedRight = true;
                    }
                    else if (rectA.Contains(new GameCoordinate(rectB.Left + rectB.Size.X / 4, rectB.Bottom + rectB.Size.Y / 4)))
                    {
                        bs.BlockedBottom = bs.BlockedRight = true;
                    }

                }
                collider.BlockedSides = bs;
            }

        }

    }
    
}
