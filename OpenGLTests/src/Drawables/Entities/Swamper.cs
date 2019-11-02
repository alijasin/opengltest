using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Swamper : Hostile
    {
        public Swamper(GameCoordinate location) : base()
        {
            this.Location = location;
            this.Animation = new Animation(new SpriteSheet_Swamper());
            this.Color = Color.PaleGreen;
            this.ActionPattern = new SwamperTeleportPattern(this, new GLCoordinate(0.3f, 0.3f));
            this.ActionPattern.Loop = true;
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.HitPoints = 1;
        }

        private bool teleported = false;

        public override void OutOfCombatStep()
        {
            //base.OutOfCombatStep(); dont do it
            if (Animation.GetSprite().sid == SpriteID.burrowing_swamper_f7)
            {
                if (teleported == false)
                {
                    ActionPattern.DoAction(OutOfCombatIndex);
                }
                teleported = true;
            }

            if (Animation.GetSprite().sid == SpriteID.burrowing_swamper_f8) teleported = false;
        }
    }
}
