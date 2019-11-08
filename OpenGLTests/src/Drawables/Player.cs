using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public class Player
    {
        public Camera ActiveCamera { get; set; }
        public static Cursor Cursor { get; set; }
        public FollowCamera FollowCamera;
        public MovableCamera StaticCamera;
        public FightCamera FightCamera;
        public HybridCamera HybridCamera;

        public Hero Hero { get; set; }
        public bool Fighting => Hero.InCombat;

        public Player(Hero Hero)
        {
            Cursor = new Cursor(this);
            Hero.Player = this;
            this.Hero = Hero;
            FollowCamera = new FollowCamera(Hero);
            StaticCamera = new MovableCamera(Hero.Location);
            HybridCamera = new HybridCamera(Hero);
            FightCamera = new FightCamera();
            ActiveCamera = HybridCamera;
        }


        public void SetFightCamera(Fight f)
        {
            FightCamera.SetFight(f);
            ActiveCamera = FightCamera;
        }

        public void SetCameraToDefault()
        {
            ActiveCamera = HybridCamera;
        }
    }
}
