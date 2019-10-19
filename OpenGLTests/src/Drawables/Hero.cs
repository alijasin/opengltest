using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenGLTests.src;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Entity, IActionCapable, ICombatable
    {
        public Inventory Inventory;
        public ActionHandler ActionHandler { get; set; }
        public bool InCombat { get; set; } = false;
        private bool waitingForActionCommit = true;
        private bool phasing = true;
        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            ActionHandler = new OutOfCombatActionHandler(this);
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            this.Animation = new Animation(new SpriteSheet_ElfIdle());

            System.Timers.Timer t = new System.Timers.Timer(2000);
            t.Elapsed += (sender, args) => { phasing = false; t.Dispose(); };
            t.Start();

            initGUI();
        }

        private void initGUI()
        {
            var actionBar = new ActionBar(this);
            GameState.Drawables.Add(actionBar);
            actionBar.Add(new Move(this));
            actionBar.Add(new Yell(this));
            actionBar.Add(new Teleport(this));
            actionBar.Add(new TossBomb(this));

            Button b = new Button();
            b.Location = new GLCoordinate(1, 1);
            b.OnInteraction += () =>
            {
                waitingForActionCommit = false;
            };
            GameState.Drawables.Add(b);

            Inventory = new Inventory(this);
            GameState.Drawables.Add(Inventory);
            Inventory.Add(new GrowingPoition(this));
            Inventory.Add(new RedPotion(this));
            Inventory.Add(new Apple(this));
        }

        public void SetCombat(bool inCombat)
        {
            if (phasing) return;
            InCombat = inCombat;
            ActionHandler.Dispose();
            actionIndex = 0;
            if (InCombat)
            {
                Console.WriteLine("entered combat");
                ActionHandler = new CombatActionHandler(this);
                waitingForActionCommit = true;
            }
            else
            {
                ActionHandler = new OutOfCombatActionHandler(this);
            }
        }

        private int actionIndex = 0;
        public void Step()
        {
            if (InCombat) CombatStep();
            else OutOfCombatStep();
        }

        private void CombatStep()
        {
            if (waitingForActionCommit) return;

            var status = ActionHandler.CommitActions(actionIndex);

            if (status == ActionReturns.Placing) return;
            if (status == ActionReturns.Finished)
            {
                actionIndex = 0;
                return;
            }
            else if (status == ActionReturns.AllFinished)
            {
                actionIndex = 0;
                waitingForActionCommit = true;
            }
            else actionIndex++;
        }

        private void OutOfCombatStep()
        {
            var status = ActionHandler.CommitActions(actionIndex);

            if (status == ActionReturns.NoAction) return;
   
            if (status == ActionReturns.Finished || status == ActionReturns.AllFinished)
            {
                actionIndex = 0;
                return;
            }
            else actionIndex++;
        }
    }
}
