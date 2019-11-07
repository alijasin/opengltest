﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenGLTests.src;
using OpenGLTests.src.Drawables.Elements;
using OpenGLTests.src.Drawables.Entities;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Unit, IActionCapable
    {
        //todo put all these things into some wrapper thing
        public int BaseAvailableActionPoints { get; set; } = 5;
        private int actionPointsAvailableAtStartOfTurn { get; set; } = 5;
        public int Stamina { get; set; } = 2;
        public InventoryButton InventoryButton;
        private Inventory Inventory;
        public Player Player { get; set; }
        public ActionBar ActionBar { get; set; }
        private ActionPointBar ActionPointBar { get; set; }
        private HashSet<Unit> AggroFrom = new HashSet<Unit>();
        private CombatTurnConfirmationButton ctcb;
        public RangeShape AggroShape { get; set; }


        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.2f, 0.1f);
            this.Speed = InitialSpeed;
            this.Animation = new Animation(new SpriteSheet_ElfIdle());
            this.ActionHandler = new OutOfCombatActionHandler(this);
            this.Initiative = 10;
            this.HitPoints = 5;
            this.Weapon = new Katana(this);
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
            ActionBar.Add(new Hook(this));
            ActionBar.Add(new HookShot(this));
            ActionBar.Add(new SpawnBearTrap(this));
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
            InventoryButton = new InventoryButton(new GLCoordinate(-1, ActionBar.Location.Y), Inventory);

            GameState.Drawables.Add(InventoryButton);
            //GameState.Drawables.Add(new HeartBar(new GLCoordinate(0, -0.86f)));
            ActionPointBar = new ActionPointBar(new GLCoordinate(new GLCoordinate(ActionBar.Location.X, ActionBar.Location.Y + ActionBar.Size.Y/2)), new GLCoordinate(ActionBar.Size.X, 0.03f));
            this.AvailableActionPoints = 5; //needs to be after ACTIONPOINTSBAR for it to be updated properly initially
            
        }

        private void ResetDefaultActionToMove()
        {
            if (ActionHandler.SelectedAction != null) ActionHandler.SelectedAction.RangeShape.Visible = false;
            ActionBar.GetDefaultButton().OnInteraction.Invoke();
            if (!InCombat) ActionHandler.SelectedAction.RangeShape.IsInfinite = true;//set it to infinite range
        }

        public override int AvailableActionPoints
        {
            set
            {
                base.AvailableActionPoints = value;
                if (base.AvailableActionPoints > 10) AvailableActionPoints = 10;
                if(ActionPointBar != null) ActionPointBar.OnAvailableActionPointsChanged(new ActionPointData(actionPointsAvailableAtStartOfTurn, Stamina, AvailableActionPoints));
            }
        }


        public override GameCoordinate LeftHandLocation()
        {
            if (this.Facing == Facing.Left)
            {
                return new GameCoordinate(this.Location.X , this.Location.Y + this.Size.Y/1.8f );
            }
            else
            {
                return new GameCoordinate(this.Location.X + this.Size.X/2, this.Location.Y - this.Size.Y/2);
            }

        }

        public override void OnDeath()
        {
            Console.WriteLine("hero died!!!!");
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
                if (AvailableActionPoints == 0)
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
            AvailableActionPoints += Stamina;
            actionPointsAvailableAtStartOfTurn = AvailableActionPoints;
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

            AvailableActionPoints = BaseAvailableActionPoints;

            Player.SetCameraToDefault();
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

            ResetDefaultActionToMove();
        }
    }
}
