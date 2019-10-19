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
        private ActionBar ActionBar { get; set; }
        public int HitPoints { get; set; } = 1;
        public void OnDeath()
        {
            Console.WriteLine("hero died!!!!");
        }

        private bool InCombat { get; set; }

        bool ICombatable.InCombat
        {
            get
            {
                return this.InCombat;
            }
            set
            {
                if (this.InCombat == value) return; //already set in combat
                this.InCombat = value;
                ActionHandler.Dispose();
                actionIndex = 0;
                if (InCombat)
                {
                    Console.WriteLine("entered combat");
                    var defaultAction = ActionBar.GetDefaultButton().GameAction;
                    defaultAction.RangeShape.IsInfinite = false; //assumes that the default action sh ouldnt be infinite in combat. 
                    ActionHandler = new CombatActionHandler(this);
                    waitingForActionCommit = true;
                }
                else
                {
                    Console.WriteLine("left combat");
                    var defaultAction = ActionBar.GetDefaultButton().GameAction;
                    defaultAction.RangeShape.IsInfinite = true;
                    ActionHandler = new OutOfCombatActionHandler(this);
                }
            }
        }

        private bool waitingForActionCommit = true;

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            this.Animation = new Animation(new SpriteSheet_ElfIdle());

            initActionBar();

            ActionHandler = new OutOfCombatActionHandler(this);
            InCombat = false;

            //set default action to first action button in the action bar
            ActionBar.GetDefaultButton().OnInteraction.Invoke();

            initGUI();
        }

        private void initActionBar()
        {
            ActionBar = new ActionBar(this);
            GameState.Drawables.Add(ActionBar);
            ActionBar.Add(new Move(this));
            ActionBar.Add(new Yell(this));
            ActionBar.Add(new Teleport(this));
            ActionBar.Add(new TossBomb(this));
        }

        private void initGUI()
        {

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
