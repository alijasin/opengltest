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
        public Swamper(GameCoordinate location)
        {
            this.AggroShape.Visible = false;
            this.Location = location;
            this.Animation = new Animation(new SpriteSheet_Swamper());
            this.Color = Color.PaleGreen;
            this.ActionPattern = new TeleportPattern(this, new GLCoordinate(0.3f, 0.3f));
            this.ActionPattern.Loop = true;
            this.AggroShape = new RangeCircle(new GLCoordinate(0.2f, 0.2f), this);
        }

        private bool teleported = false;

        public override void OutOfCombatStep()
        {
            //base.OutOfCombatStep(); dont do it
            if (Animation.GetSprite().sid == SpriteID.burrowing_swamper_f7)
            {
                if (teleported == false)
                {
                    ActionPattern.DoAction("ogeli");
                }
                teleported = true;
            }

            if (Animation.GetSprite().sid == SpriteID.burrowing_swamper_f8) teleported = false;
        }
    }
}
