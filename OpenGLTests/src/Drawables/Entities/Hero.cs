using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenGLTests.src;
using OpenGLTests.src.Drawables.Elements;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Unit, IActionCapable
    {
        public Inventory Inventory;
        private ActionBar ActionBar { get; set; }
        public int HitPoints { get; set; } = 1;
        private HashSet<Unit> AggroFrom = new HashSet<Unit>();
        private CombatTurnConfirmationButton ctcb;

        private void ResetDefaultActionToMove()
        {
            ActionBar.GetDefaultButton().OnInteraction.Invoke();
            if(!InCombat)ActionHandler.SelectedAction.RangeShape.IsInfinite = true;//set it to infinite range
        }


        public void Damage(int dmg)
        {
            
        }

        public void OnDeath()
        {
            Console.WriteLine("hero died!!!!");
        }


        public RangeShape AggroShape { get; set; }

        private bool waitingForActionCommit = true; //todo remove this and call the commit from interaction button directly.

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            this.Animation = new Animation(new SpriteSheet_ElfIdle());
            this.ActionHandler = new OutOfCombatActionHandler(this);
            this.Initiative = 10;
            //this.AggroShape = new RangeCircle(new GLCoordinate(0, 0), this);
            initActionBar();
            InCombat = false;
            
            ResetDefaultActionToMove();
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

        private void initInventory()
        {
            //todo
        }

        private void initGUI()
        {
            ctcb = new CombatTurnConfirmationButton(new GLCoordinate( 0.9f, 0.9f));
            ctcb.OnInteraction += () =>
            {
                if (InCombat && CommitedActions == false)
                {
                    CommitedActions = true;
                }
            };
            GameState.Drawables.Add(ctcb);

            Inventory = new Inventory(this);
            GameState.Drawables.Add(Inventory);
            Inventory.Add(new GrowingPoition(this));
            Inventory.Add(new RedPotion(this));
            Inventory.Add(new Apple(this));


            //GameState.Drawables.Add(new HeartBar(new GLCoordinate(0, -0.86f)));
        }

        public override void OutOfCombatStep()
        {
            var status = ActionHandler.CommitActions(OutOfCombatIndex);
            if (status == ActionReturns.NoAction) return;

            if (status == ActionReturns.Finished || status == ActionReturns.AllFinished)
            {
                OutOfCombatIndex = 0;
                ResetDefaultActionToMove();
                return;
            }
            else OutOfCombatIndex++;
        }

        public override void OnPreTurn()
        {
            ctcb.Enabled = true;
        }

        //called from Fight public void UnitFinishedTurn(Unit u)
        public override void OnPostTurn()
        {
            ResetDefaultActionToMove();
            if(AggroFrom.Count > 0) ctcb.Enabled = false;
        }

        public void Deaggro(Unit deAggroed)
        {
            Console.WriteLine("hero deaggroed " + deAggroed);
            AggroFrom.Remove(deAggroed);
            if (AggroFrom.Count == 0)
            {
                OnLeftCombat();
            }
        }

        public void OnLeftCombat()
        {
            InCombat = false;

            Console.WriteLine("left combat");
            var defaultAction = ActionBar.GetDefaultButton().GameAction;
            defaultAction.RangeShape.IsInfinite = true;
            ActionHandler.Dispose();
            ActionHandler = new OutOfCombatActionHandler(this);
            ResetDefaultActionToMove();
            ctcb.Enabled = true;
        }

        public override void OnAggro(Unit aggroed)
        {
            Console.WriteLine("hero aggroed " + aggroed);
            AggroFrom.Add(aggroed);
            InCombat = true;

            ActionHandler.Dispose();

            var defaultAction = ActionBar.GetDefaultButton().GameAction;
            defaultAction.RangeShape.IsInfinite = false; //assumes that the default action sh ouldnt be infinite in combat. 
            ActionHandler.Dispose();
            ActionHandler = new CombatActionHandler(this);
            waitingForActionCommit = true;
            ResetDefaultActionToMove();
        }
    }
}
