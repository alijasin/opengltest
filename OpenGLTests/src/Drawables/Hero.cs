using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Entity, IActor
    {
        public ActionHandler ActionHandler { get; set; }
       // public GameAction ActiveAction { get; set; }

        //public LinkedList<Action> CommitedActions { get; set; }
        private bool ExecutingActions = false;
        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            ActionHandler = new ActionHandler(this);
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            initGUI();
        }

        private void initGUI()
        {
            MoveAction ma = new MoveAction(new GLCoordinate(0.3f, 0.3f), this);
            ActionHandler.AddNewAvailableAction(ma);


            LambdaAction la = new LambdaAction((o) =>
            {
                Console.Write(o);
                Console.WriteLine("big boi");
                return true;
            });
            ActionHandler.AddNewAvailableAction(la);

            TeleportAction ca = new TeleportAction(new GLCoordinate(0.5f, 0.5f), this);
            ActionHandler.AddNewAvailableAction(ca);

            AOEEffectAction aoe = new AOEEffectAction(new GLCoordinate(0.6f, 0.6f),new GLCoordinate(0.2f, 0.2f));
            ActionHandler.AddNewAvailableAction(aoe);

            Button b = new Button();
            b.Location = new GLCoordinate(1, 1);
            b.OnInteraction += () =>
            {
                ExecutingActions = true;
            };
            GameState.Drawables.Add(b);
        }
        


        public override void Step()
        {
            if (ExecutingActions)
            {
                CombatStep();
            }
        }

        private static int index = 0;
        public override void CombatStep()
        {
            base.CombatStep();
            ActionHandler.ActionReturns res = ActionHandler.TickPlacedActions(index);
            if (res == ActionHandler.ActionReturns.AllFinished)
            {
                index = 0;
                ExecutingActions = false;
            }
            else if (res == ActionHandler.ActionReturns.Ongoing)
            {
                index++;
            }
            else if(res == ActionHandler.ActionReturns.Finished)
            {
                index = 0;
            }
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
        }
    }
}
