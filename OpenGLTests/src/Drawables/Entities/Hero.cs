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
        public int InitialActionPoint { get; set; } = 3;
        public int ActionPoints { get; set; } = 3;
        public int Stamina { get; set; } = 2;
        public Inventory Inventory;
        public ActionBar ActionBar { get; set; }
        public int HitPoints { get; set; } = 1;
        private HashSet<Unit> AggroFrom = new HashSet<Unit>();
        private CombatTurnConfirmationButton ctcb;

        private void ResetDefaultActionToMove()
        {
            if(ActionHandler.SelectedAction != null) ActionHandler.SelectedAction.RangeShape.Visible = false;
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
            ActionBar.Add(new HookShot(this));
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
                if (InCombat && EndedTurn == false)
                {
                    EndedTurn = true;
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

        public override void CombatStep(Fight fight)
        {
            if (EndedTurn)
            {
                fight.UnitFinishedTurn(this);
                return;
            }
            ActionStatus = ActionHandler.CommitActions(CombatIndex);

            if (ActionStatus == ActionReturns.Placing) return;
            if (ActionStatus == ActionReturns.Finished || ActionStatus == ActionReturns.AllFinished)
            {
                CombatIndex = 0;
                ActionPoints--;
                if (ActionPoints == 0)
                {
                    Console.WriteLine(this + " ran out of action points. Force ending turn.");
                    EndedTurn = true;
                }
                return;
            }
            else if(ActionStatus == ActionReturns.Ongoing) CombatIndex++;

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

            CombatIndex = 0;
            EndedTurn = false;
            ActionPoints += Stamina;
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
            ctcb.Visible = false;
            Console.WriteLine("left combat");
            var defaultAction = ActionBar.GetDefaultButton().GameAction;
            defaultAction.RangeShape.IsInfinite = true;
            ActionHandler.Dispose();
            ActionHandler = new OutOfCombatActionHandler(this);
            ResetDefaultActionToMove();
            ctcb.Enabled = true;

            ActionPoints = InitialActionPoint;
        }

        public override void OnAggro(Unit aggroed)
        {
            Console.WriteLine("hero aggroed " + aggroed);
            AggroFrom.Add(aggroed);
            InCombat = true;
            ctcb.Visible = true;
            ActionHandler.Dispose();

            var defaultAction = ActionBar.GetDefaultButton().GameAction;
            defaultAction.RangeShape.IsInfinite = false; //assumes that the default action sh ouldnt be infinite in combat. 
            ActionHandler.Dispose();
            ActionHandler = new CombatActionHandler(this);
            waitingForActionCommit = true;
            ResetDefaultActionToMove();
        }
        /*
        public void TestOnClick(GameCoordinate xd)
        {
            clicked = new GameCoordinate(xd.X, xd.Y);
        }
        GameCoordinate clicked = new GameCoordinate(0, 0);
        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            drawer.DrawFan(this.Location.ToGLCoordinate(), clicked, 0.5f, 60);
        }*/
    }
}
