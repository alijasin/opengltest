using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OpenGLTests.src
{
    public class GameController
    {
        public const int StepTickSpeed = 60;

        public GameState Game { get; private set; }

        private Timer StepTimer { get; set; }

        public void StartGame()
        {
            var gscreen = new GameScreen();
            GraphicsController.ActiveScreen = gscreen;
            Game = gscreen.Game;
            StepTimer = new Timer(1000.0 / StepTickSpeed);
            StepTimer.Elapsed += StepTimerElapsed;
            StepTimer.Start();
        }


        public void EndGame()
        {
            if (Game == null)
            { 
                return;
            }

            StepTimer.Stop();
            StepTimer = null;
            Game = null;
        }

        private void StepTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Game.Step();
        }
    }

    public class EditorController
    {
        public void Init()
        {
            var gscreen = new EditorScreen();
            GraphicsController.ActiveScreen = gscreen;
        }
    }
}
