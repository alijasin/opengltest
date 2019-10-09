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

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            ActionHandler = new ActionHandler(this);

            initGUI();
        }

        private void initGUI()
        {
            MoveAction ma = new MoveAction(new GLCoordinate(0.3f, 0.3f), this);
            ActionHandler.AddNewAvailableAction(ma);


            LambdaAction la = new LambdaAction(() =>
            {
                Console.WriteLine("big boi");
            });
            ActionHandler.AddNewAvailableAction(la);

            ChargeAction ca = new ChargeAction(new GLCoordinate(0.2f, 0.2f), this);
            ActionHandler.AddNewAvailableAction(ca);


            Button b = new Button();
            b.Location = new GLCoordinate(1, 1);
            b.OnInteraction += () =>
            {
                GameState.CombatStep();
            };
            GameState.Drawables.Add(b);
        }
        


        public override void Step()
        {
            base.Step();
            this.Location += this.Speed;
        }

        public override void CombatStep()
        {
            base.CombatStep();
            ActionHandler.DoCommitedActions();
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
        }
    }
}
