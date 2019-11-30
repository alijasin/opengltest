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
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Unit, ICollidable
    {

        //todo put all these things into some wrapper thing
        public int BaseAvailableActionPoints { get; set; } = 5;
        private int actionPointsAvailableAtStartOfTurn { get; set; } = 5;
        public int Stamina { get; set; } = 2;
        public InventoryButton InventoryButton;
        public Inventory Inventory { get; set; }
        public Player Player { get; set; }
        public ActionBar ActionBar { get; set; }
        private ActionPointBar ActionPointBar { get; set; }
        private HashSet<Unit> AggroFrom = new HashSet<Unit>();
        private CombatTurnConfirmationButton ctcb;
        public RangeShape AggroShape { get; set; }
        public EquipmentHandler EquipmentHandler { get; set; }
        public bool Phased { get; set; } = true;
        public RangeShape BoundingBox { get; set; }

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            this.Speed = InitialSpeed;
            this.Animation = new Animation(new SpriteSheet_ElfIdle());
            this.ActionHandler = new OutOfCombatActionHandler(this);
            this.Initiative = 10;
            this.HitPoints = 5;
            //this.Weapon = new Katana(this);
            

            EquipmentHandler = new EquipmentHandler(this);
            BoundingBox = new RangeShape(new Rectangle(this.Size), this);
            initActionBar();
            this.DefaultAction = ActionBar.GetDefaultButton().GameAction;
            InCombat = false;
            initGUI();
        }

        public override int FacingAngle
        {
            get { return MyMath.AngleBetweenTwoPoints(this.Location, Player.Cursor.Location); }
        }

        public override Weapon Weapon
        {
            set
            {
                base.Weapon = value;
                this.ActionBar.Swap(base.Weapon);
            }
        }

        private void initActionBar()
        {
            ActionBar = new ActionBar(this);
            GameState.Drawables.Add(ActionBar);
            ActionBar.Add(new MoveAbility(this));
            ActionBar.Add(new UnarmedAbility(this));
            ActionBar.Add(new TeleportAbility(this));
            ActionBar.Add(new HookAbility(this));
            ActionBar.Add(new HookShotAbility(this));
            ActionBar.Add(new SpawnBearTrapAbility(this));
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
            Inventory.Add(new GrowingPotion(this));
            Inventory.Add(new RedPotion(this));
            Inventory.Add(new Apple(this));
            Inventory.Add(new Apple(this));
            Inventory.Add(new Apple(this));
            InventoryButton = new InventoryButton(new GLCoordinate(-1, ActionBar.Location.Y), Inventory);

            GameState.Drawables.Add(InventoryButton);
            //GameState.Drawables.Add(new HeartBar(new GLCoordinate(0, -0.86f)));
            ActionPointBar = new ActionPointBar(new GLCoordinate(new GLCoordinate(ActionBar.Location.X, ActionBar.Location.Y + ActionBar.Size.Y/2)), new GLCoordinate(ActionBar.Size.X, 0.03f));
            this.AvailableActionPoints = 5; //needs to be after ACTIONPOINTSBAR for it to be updated properly initially
            
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

        public override GameCoordinate LeftHandLocation
        {
            get
            {
                if (DoingWeaponAction)
                {
                    return new GameCoordinate(this.Location.X, this.Location.Y);
                }

                if (this.Facing == Facing.Left)
                {
                    return new GameCoordinate(this.Location.X, this.Location.Y + this.Size.Y / 1.8f);
                }
                else
                {
                    return new GameCoordinate(this.Location.X + this.Size.X / 2, this.Location.Y - this.Size.Y / 2);
                }
            }
            set { }
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

            if (ActionStatus == src.ActionStatus.Placing) return;
            if (ActionStatus == src.ActionStatus.Finished || ActionStatus == src.ActionStatus.AllFinished)
            {
                CombatIndex = 0;
                if (AvailableActionPoints == 0)
                {
                    Console.WriteLine(this + " ran out of action points. Force ending turn.");
                    EndedTurn = true;
                }
                return;
            }
            else if(ActionStatus == src.ActionStatus.Ongoing) CombatIndex++;

        }

        public override void OutOfCombatStep()
        {
            ActionStatus = ActionHandler.CommitActions(OutOfCombatIndex);
            if (ActionStatus == src.ActionStatus.NoAction) return;
            if (ActionStatus == src.ActionStatus.Finished || ActionStatus == src.ActionStatus.AllFinished)
            {
                OutOfCombatIndex = 0;
                ActionHandler.ClearSelected();
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
            ActionHandler.ClearSelected();
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
            ActionHandler.ClearSelected();
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

            ActionHandler.ClearSelected();
        }
    }
}
