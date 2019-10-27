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
        private HashSet<ICombatable> AggroFrom = new HashSet<ICombatable>();

        private void ResetDefaultActionToMove()
        {
            ActionBar.GetDefaultButton().OnInteraction.Invoke();
            if(!InCombat)ActionHandler.SelectedAction.RangeShape.IsInfinite = true;//set it to infinite range
        } 


        public void OnDeath()
        {
            Console.WriteLine("hero died!!!!");
        }

        public RangeShape AggroShape { get; set; }
        public bool InCombat { get; set; }
        private bool waitingForActionCommit = true; //todo remove this and call the commit from interaction button directly.

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            this.Animation = new Animation(new SpriteSheet_ElfIdle());
            //this.AggroShape = new RangeCircle(new GLCoordinate(0, 0), this);
            initActionBar();

            ActionHandler = new OutOfCombatActionHandler(this);
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
                ResetDefaultActionToMove();
                return;
            }
            else actionIndex++;
        }

        public void Deaggro(ICombatable deAggroed)
        {
            Console.WriteLine("hero deaggroed " + deAggroed);
            AggroFrom.Remove(deAggroed);
            if (AggroFrom.Count == 0)
            {
                InCombat = false;

                Console.WriteLine("left combat");
                var defaultAction = ActionBar.GetDefaultButton().GameAction;
                defaultAction.RangeShape.IsInfinite = true;
                ActionHandler.Dispose();
                ActionHandler = new OutOfCombatActionHandler(this);
                ResetDefaultActionToMove();
            }
        }

        public void OnAggro(ICombatable aggroed)
        {
            Console.WriteLine("hero aggroed " + aggroed);
            AggroFrom.Add(aggroed);
            InCombat = true;

            ActionHandler.Dispose();

            //if we are in aggro already and we aggro another one this will reset. Todo:
            actionIndex = 0;
            var defaultAction = ActionBar.GetDefaultButton().GameAction;
            defaultAction.RangeShape.IsInfinite = false; //assumes that the default action sh ouldnt be infinite in combat. 
            ActionHandler.Dispose();
            ActionHandler = new CombatActionHandler(this);
            waitingForActionCommit = true;
            ResetDefaultActionToMove();
        }
    }
}
