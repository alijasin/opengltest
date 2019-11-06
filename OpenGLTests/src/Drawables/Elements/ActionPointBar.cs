using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables.Elements
{
    class ActionPoint : Element
    {
        enum State
        {
            used,
            unused,
            tobefilled,
            locked,
        }

        private State state;
        public ActionPoint()
        {
            this.Animation = new Animation(new SpriteSheet_ActionPointBarStuff());
            this.Animation.SetSprite(SpriteID.ui_action_point_unused);
            this.Animation.IsStatic = true;
            this.state = State.locked;
        }
    }
    class ActionPointBar : Element
    {
        private const int NUMBER_OF_ACTION_POINTS = 10;
        private ActionPoint [] actionPoints = new ActionPoint[NUMBER_OF_ACTION_POINTS];
        
        public ActionPointBar()
        {
            this.Location = new GLCoordinate(0, 0);
            this.Size = new GLCoordinate(0.8f, 0.03f);
            this.Animation = new Animation(new SpriteSheet_ActionPointBarStuff());
            this.Animation.SetSprite(SpriteID.ui_action_point_bar);
            this.Animation.IsStatic = true;
            for (int i = 0; i < NUMBER_OF_ACTION_POINTS; i++)
            {
                actionPoints[i] = new ActionPoint();
                actionPoints[i].Size = new GLCoordinate(this.Size.X / NUMBER_OF_ACTION_POINTS - this.Size.X/NUMBER_OF_ACTION_POINTS*0.1f, 0.1f);
                actionPoints[i].Location = new GLCoordinate(this.Location.X - this.Size.X/2 + this.Size.X/NUMBER_OF_ACTION_POINTS*i + actionPoints[i].Size.X/2, this.Location.Y + this.Size.Y/2 + actionPoints[i].Size.Y/2);
            }
            GameState.Drawables.Add(this);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            foreach (var ap in actionPoints)
            {
                ap.DrawStep(drawer);
            }
        }
    }
}
