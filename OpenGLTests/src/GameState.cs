﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Entities;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class GameState
    {
        public Hero Hero { get; set; }
        public static Camera ActiveCamera { get; set; }
        public static bool CombatMode { get; set; } = true;
        public static Drawablex Drawables = new Drawablex();
        public static List<IInteractable> Interactables = new List<IInteractable>();

        public GameState()
        {
            Hero = new Hero();
            Drawables.Add(Hero);

            var followCamera = new FollowCamera(Hero);
            var staticCamera = new StaticCamera(Hero.Location);
            ActiveCamera = staticCamera;
            Button testbutton = new Button();
            testbutton.Location = new GLCoordinate(-1, 1);
            testbutton.OnInteraction = () =>
            {
                Console.WriteLine("Camera swap");
                if (ActiveCamera is StaticCamera)
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

        public void Step()
        {
            var entities = Drawables.Get.Where(d => d is Entity).ToList();
            foreach (var e in entities)
            {
                e.Step();
            }
            ActiveCamera.Step();
        }

        public static void CombatStep()
        {
            var drawables = Drawables.Get.ToList();
            foreach (var e in drawables)
            {
                e.CombatStep();
            }
        }
    }
}
