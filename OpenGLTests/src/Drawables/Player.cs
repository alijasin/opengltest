using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public class Player
    {
        public static Cursor Cursor { get; set; }
        public FollowCamera FollowCamera;
        public MovableCamera StaticCamera;
        public FightCamera FightCamera;
        public HybridCamera HybridCamera;
        public Camera ActiveCamera => Camera.ActiveCamera;
        public Hero Hero { get; set; }
        public bool Fighting => Hero.InCombat;

        public Player(Hero Hero)
        {
            Cursor = new Cursor(this);
            Hero.Player = this;
            this.Hero = Hero;
            this.Hero.ActionHandler.ClearSelected();
            FollowCamera = new FollowCamera(Hero);
            StaticCamera = new MovableCamera(Hero.Location);
            HybridCamera = new HybridCamera(Hero);
            FightCamera = new FightCamera();
            Camera.ActiveCamera = HybridCamera;
        }


        public void SetFightCamera(Fight f)
        {
            FightCamera.SetFight(f);
            Camera.ActiveCamera = FightCamera;
        }

        public void SetCameraToDefault()
        {
            Camera.ActiveCamera = HybridCamera;
        }
    }
}
