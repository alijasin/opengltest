﻿using System;
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
        public Hero Hero { get; set; }
        public static DrawableRepository Drawables = new DrawableRepository();
        public static bool Combat { get; set; } = false;
        public static RainGenerator RainGenerator = new RainGenerator(RainGenerator.RainType.Clear); //todo: move to drawable
        public GameState()
        {
            for (int x = -10; x < 20; x++)
            {
                for (int y = -10; y < 20; y++)
                {
                  //  GameState.Drawables.Add(new Floor(new GameCoordinate(0.1f * x, 0.1f * y)));
                }
            }
            Hero = new Hero();
            Drawables.Add(Hero);
            Drawables.Add(new RoomLoadRegion(new GameCoordinate(0.8f, 0.8f), RoomLoader.Room.TestSpace));
   

            RoomLoader.LoadRoom(RoomLoader.Room.TestEditorOutPut);

            //todo refactor this into screen
            var followCamera = new FollowCamera(Hero);
            var staticCamera = new MovableCamera(Hero.Location);
            var hybridCamera = new HybridCamera(Hero);

            Screen.ActiveCamera = hybridCamera;

            Button testbutton = new Button();
            testbutton.Location = new GLCoordinate(-1, 1);
            testbutton.OnInteraction = () =>
            {
                Console.WriteLine("Camera swap");
                if (Screen.ActiveCamera is MovableCamera)
                {
                    Screen.ActiveCamera = hybridCamera;
                }
                else if(Screen.ActiveCamera is FollowCamera)
                {
                    Screen.ActiveCamera = staticCamera;
                }
                else if (Screen.ActiveCamera is HybridCamera)
                {
                    Screen.ActiveCamera = followCamera;
                }
                Screen.ActiveCamera.Location = Hero.Location;
            };
            Drawables.Add(testbutton);
        }

        //todo: create class room and let room load entities from a file.
        private void LoadRoom()
        {
            var angerdude = new AngryDude(new GameCoordinate(0.2f, 0.8f));
            Drawables.Add(angerdude);

            var patroldude = new PatrolGuy(new GameCoordinate(-0.6f, -0.4f));
            Drawables.Add(patroldude);

            var chasingdude = new ChasingPerson(new GameCoordinate(0.5f, 0));
            Drawables.Add(chasingdude);

            var swamper = new Swamper(new GameCoordinate(0.5f, -0.5f));
            Drawables.Add(swamper);

            //var unicorn = new Unicorn(new GameCoordinate(-0.5f, 0), Hero);
            //Drawables.Add(unicorn);

        }

        private void LoadTestRoom()
        {
            RoomLoader rl = new RoomLoader();
        }

        private Fight fight = new Fight();
        private int initSteps = 25;
        private int initStepsCount = 0;
        private static int outOfCombatIndex = 0;
        public void Step()
        {
            Screen.ActiveCamera.Step();
            if (initStepsCount < initSteps)
            {
                initStepsCount++;
                return;
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

            #region Pause for Fight

            //if we have units in action do fight steps
            //else we do out of combat actions. 
            //this means units not in combat when there is a fight going on will be paused.
            /*if (!fight.LastManStanding()) doFight();
            else
            {
                foreach (var unit in Drawables.GetAllUnits)
                {

                    unit.OutOfCombatStep(outOfCombatIndex);

                }
            }
            */

            #endregion
            if (!fight.LastManStanding()) doFight();

            foreach (var unit in Drawables.GetAllUnits)
            {
                if (unit.InCombat == false)
                {
                    unit.OutOfCombatStep(outOfCombatIndex);
                }
            }


            foreach (Unit aggro in Drawables.GetAllUnits.Where(c => !(c is Hero) && c.AggroShape != null && c.InCombat == false).ToList())
            {
                if (aggro.AggroShape.Contains(Hero.Location))
                {
                    aggro.OnAggro(Hero);
                    Hero.OnAggro(aggro);
                    fight.AddFighter(Hero);
                    fight.AddFighter(aggro);
                }
            }
        }

        private static int combatIndex = 0;
        private void doFight()
        {
            var currentFighter = fight.GetCurrentTurn();
            if (currentFighter != null)
            {
                if (currentFighter is Hero h)
                {
                    if (h.CommitedActions == false) return;
                }

                var status = currentFighter.ActionHandler.CommitActions(combatIndex);

                if (status == ActionReturns.Placing) return;
                if (status == ActionReturns.Finished)
                {
                    combatIndex = 0;
                    return;
                }
                else if (status == ActionReturns.AllFinished)
                {
                    combatIndex = 0;
                    currentFighter.CommitedActions = false;
                    fight.UnitFinishedTurn(currentFighter);
                }
                else combatIndex++;
            }
        }
    }
}
