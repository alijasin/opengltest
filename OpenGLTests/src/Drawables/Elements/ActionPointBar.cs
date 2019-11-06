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
        public enum State
        {
            Used,
            Unused,
            ToBeFilled,
            Locked,
        }

        private State state;
        public ActionPoint()
        {
            this.Animation = new Animation(new SpriteSheet_ActionPointBarStuff());
            this.Animation.SetSprite(SpriteID.ui_action_point_unused);
            this.Animation.IsStatic = true;
            this.state = State.Locked;
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.Unused: this.Animation.SetSprite(SpriteID.ui_action_point_unused);
                    break;
                case State.Used: this.Animation.SetSprite(SpriteID.ui_action_point_used);
                    break;
                case State.ToBeFilled: this.Animation.SetSprite(SpriteID.ui_action_point_tobefilled);
                    break;
                case State.Locked: this.Animation.SetSprite(SpriteID.ui_action_point_locked);
                    break;
            }

            this.state = state;
        }
    }

    class GoldenThing : Element
    {
        public GoldenThing()
        {
            this.Animation = new Animation(new SpriteSheet_ActionPointBarStuff());
            this.Animation.SetSprite(SpriteID.ui_action_point_golden);
            this.Animation.IsStatic = true;
        }
    }

    class ActionPointBar : Element
    {
        private const int NUMBER_OF_ACTION_POINTS = 10;
        private ActionPoint [] actionPoints = new ActionPoint[NUMBER_OF_ACTION_POINTS];
        private GoldenThing [] goldenThings = new GoldenThing[2];

        private float leftMost => this.Location.X - this.Size.X / 2;
        private float topMost => this.Location.Y + this.Size.Y/2;
        public ActionPointBar(GLCoordinate location, GLCoordinate size)
        {
            this.Size = size;
            this.Location = new GLCoordinate(location.X, location.Y + this.Size.Y/2);
            this.Animation = new Animation(new SpriteSheet_ActionPointBarStuff());
            this.Animation.SetSprite(SpriteID.ui_action_point_bar);
            this.Animation.IsStatic = true;
            for (int i = 0; i < NUMBER_OF_ACTION_POINTS; i++)
            {
                actionPoints[i] = new ActionPoint();
                actionPoints[i].Size = new GLCoordinate(this.Size.X / NUMBER_OF_ACTION_POINTS - this.Size.X/NUMBER_OF_ACTION_POINTS*0.1f, this.Size.Y * 2);
                actionPoints[i].Location = new GLCoordinate(leftMost + this.Size.X/NUMBER_OF_ACTION_POINTS*i + actionPoints[i].Size.X/2, topMost + actionPoints[i].Size.Y/2 - 0.008f); //todo: we want to move these one pixel down but this is just insane
            }

            GoldenThing leftGolden = new GoldenThing();
            GoldenThing rightGolden = new GoldenThing();
            leftGolden.Size = rightGolden.Size = new GLCoordinate(0.02f, this.Size.Y*2);
            leftGolden.Location = new GLCoordinate(leftMost - leftGolden.Size.X / 2, this.Location.Y);
            rightGolden.Location = new GLCoordinate(leftMost + this.Size.X, this.Location.Y);
            goldenThings[0] = leftGolden;
            goldenThings[1] = rightGolden;

            GameState.Drawables.Add(this);
        }

        public void OnAvailableActionPointsChanged(ActionPointData apd)
        {
            int used = apd.TurnsInitialActionPoints - apd.AvailableActionPoints;
            for (int i = 0; i < NUMBER_OF_ACTION_POINTS; i++)
            {
                if (i < apd.AvailableActionPoints) actionPoints[i].SetState(ActionPoint.State.Unused);
                else if(i < apd.AvailableActionPoints + apd.Stamina) actionPoints[i].SetState(ActionPoint.State.ToBeFilled);
                else if(i < apd.TurnsInitialActionPoints + apd.Stamina) actionPoints[i].SetState(ActionPoint.State.Used);
                else actionPoints[i].SetState(ActionPoint.State.Locked);
            }
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            foreach (var ap in actionPoints)
            {
                ap.DrawStep(drawer);
            }

            foreach (var golden in goldenThings)
            {
                golden.DrawStep(drawer);
            }
        }
    }

    public class ActionPointData
    {
        public int TurnsInitialActionPoints;
        public int Stamina;
        public int AvailableActionPoints;
        
        public ActionPointData(int turnsInitialActionPoints, int Stamina, int availableActionPoints)
        {
            this.TurnsInitialActionPoints = turnsInitialActionPoints;
            this.Stamina = Stamina;
            this.AvailableActionPoints = availableActionPoints;
        }
    }
}
