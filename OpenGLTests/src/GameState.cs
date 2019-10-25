﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Entities;
using OpenGLTests.src.Util;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class GameState
    {
        public Hero Hero { get; set; }
        public static Camera ActiveCamera { get; set; }
        public static DrawableRepository Drawables = new DrawableRepository();
        public static bool Combat { get; set; } = false;

        public GameState()
        {
            GameConsole.Init();
            Hero = new Hero();
            Drawables.Add(Hero);

            LoadRoom();
            //LoadTestRoom();
            //RoomLoader.LoadRoom(RoomLoader.Room.RandomGenerated);
            //var MouseParticleGenerator = new TestParticleGenerator(50);
            //Drawables.Add(MouseParticleGenerator);

            var followCamera = new FollowCamera(Hero);
            var staticCamera = new MovableCamera(Hero.Location);

            ActiveCamera = staticCamera;

            Button testbutton = new Button();
            testbutton.Location = new GLCoordinate(-1, 1);
            testbutton.OnInteraction = () =>
            {
                Console.WriteLine("Camera swap");
                if (ActiveCamera is MovableCamera)
                {
                    ActiveCamera = followCamera;
                }
                else
                {
                    ActiveCamera = staticCamera;
                }
                ActiveCamera.Location = Hero.Location;
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

            var chasingdude = new ChasingPerson(new GameCoordinate(0.5f, 0), Hero);
            Drawables.Add(chasingdude);

            var swamper = new Swamper(new GameCoordinate(0.5f, -0.5f));
            Drawables.Add(swamper);

            var unicorn = new Unicorn(new GameCoordinate(-0.5f, 0), Hero);
            Drawables.Add(unicorn);

        }

        private void LoadTestRoom()
        {
            RoomLoader rl = new RoomLoader();
        }

        private int initSteps = 25;
        private int initStepsCount = 0;
        public void Step()
        {
            if (initStepsCount < initSteps)
            {
                initStepsCount++;
                return;
            }

            var combatables = Drawables.GetAllCombatables;

            foreach (ICombatable aggro in combatables.Where(c => c is ICombatable && !(c is Hero) && c.AggroShape != null && c.InCombat == false))
            {
                if (aggro.AggroShape.Contains(Hero.Location))
                {
                    aggro.OnAggro(Hero);
                    Hero.OnAggro(aggro);
                }
            }

            foreach (var combatable in combatables)
            {
                combatable.Step();
                if(combatable.HitPoints <= 0) combatable.OnDeath();
            }


            ActiveCamera.Step();
        }
    }
}
