using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Terrain;
using OpenGLTests.src.Entities;
using OpenGLTests.src.Util;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTests.src
{
    public class GameState
    {
        public Hero Hero { get; set; }
        public static DrawableRepository Drawables = new DrawableRepository();
        public static bool Combat { get; set; } = false;
        public static RainGenerator RainGenerator = new RainGenerator(RainGenerator.RainType.Heavy); //todo: move to drawable
        public GameState()
        {
            for (int x = -10; x < 20; x++)
            {
                for (int y = -10; y < 20; y++)
                {
                    GameState.Drawables.Add(new Floor(new GameCoordinate(0.1f * x, 0.1f * y)));
                }
            }
            Hero = new Hero();
            Drawables.Add(Hero);
            

            //LoadRoom();
            //LoadTestRoom();
            RoomLoader.LoadRoom(RoomLoader.Room.TestEditorOutPut);
            //var MouseParticleGenerator = new TestParticleGenerator(50);
            //Drawables.Add(MouseParticleGenerator);


            //todo refactor this into screen
            var followCamera = new FollowCamera(Hero);
            var staticCamera = new MovableCamera(Hero.Location);

            Screen.ActiveCamera = staticCamera;

            Button testbutton = new Button();
            testbutton.Location = new GLCoordinate(-1, 1);
            testbutton.OnInteraction = () =>
            {
                Console.WriteLine("Camera swap");
                if (Screen.ActiveCamera is MovableCamera)
                {
                    Screen.ActiveCamera = followCamera;
                }
                else
                {
                    Screen.ActiveCamera = staticCamera;
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

            var chasingdude = new ChasingPerson(new GameCoordinate(0.5f, 0), Hero);
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

        private int initSteps = 25;
        private int initStepsCount = 0;
        public void Step()
        {
            Screen.ActiveCamera.Step();
            if (initStepsCount < initSteps)
            {
                initStepsCount++;
                return;
            }

            object l = 1;
            lock (l)
            {
                foreach (ICombatable aggro in Drawables.GetAllCombatables.Where(c => c is ICombatable && !(c is Hero) && c.AggroShape != null && c.InCombat == false).ToList())
                {
                    if (aggro.AggroShape.Contains(Hero.Location))
                    {
                        aggro.OnAggro(Hero);
                        Hero.OnAggro(aggro);
                    }
                }

                foreach (var combatable in Drawables.GetAllCombatables)
                {
                    combatable.Step();
                    if (combatable.HitPoints <= 0) combatable.OnDeath();
                }
            }
        }
    }
}
