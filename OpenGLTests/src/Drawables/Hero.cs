using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Entity, IActor, ICombatable
    {
        public Inventory Inventory;
        public ActionHandler ActionHandler { get; set; }
        private bool ExecutingActions = false;
        public bool InCombat { get; set; }

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            ActionHandler = new ActionHandler(this);
            this.Speed = new GameCoordinate(0.02f, 0.02f);
            this.Animation = new Animation(new SpriteSheet_ElfIdle());

            initGUI();
        }

        private void initGUI()
        {
            var actionBar = new ActionBar(this);
            GameState.Drawables.Add(actionBar);
            MoveAction ma = new MoveAction(new GLCoordinate(0.3f, 0.3f), this);
            actionBar.Add(ma);

            LambdaAction la = new LambdaAction((o) =>
            {
                Console.WriteLine("big boi");
                return true;
            });
            actionBar.Add(la);

            TeleportAction ca = new TeleportAction(new GLCoordinate(0.5f, 0.5f), this);
            actionBar.Add(ca);

            AOEEffectAction aoe = new AOEEffectAction(new GLCoordinate(0.6f, 0.6f),new GLCoordinate(0.2f, 0.2f));
            actionBar.Add(aoe);

            Button b = new Button();
            b.Location = new GLCoordinate(1, 1);
            b.OnInteraction += () =>
            {
                ExecutingActions = true;
            };
            GameState.Drawables.Add(b);

            Inventory = new Inventory(this);
            GameState.Drawables.Add(Inventory);
            Inventory.Add(new GrowingPoition(this));
            Inventory.Add(new RedPotion(this));
            Inventory.Add(new Apple(this));

        }


        //todo refactror this so we dont have literally duplicated code
        private static int outOfCombatIndex = 0;
        public void OutOfCombatStep()
        {
            

            ActionReturns res = ActionHandler.OutOfCombatActionHandler.TickGameAction(outOfCombatIndex);
            if (res == ActionReturns.AllFinished)
            {
                outOfCombatIndex = 0;
            }
            else if (res == ActionReturns.Ongoing)
            {
                outOfCombatIndex++;
            }
            else if (res == ActionReturns.Finished)
            {
                outOfCombatIndex = 0;
            }
        }

        //todo refactror this so we dont have literally duplicated code
        private static int combatIndex = 0;
        public void CombatStep()
        {
            if (!ExecutingActions) return; //if you decide later than you want to get rid of action confirmation dont do this check.

            ActionReturns res = ActionHandler.CombatActionHandler.TickPlacedActions(combatIndex);
            if (res == ActionReturns.AllFinished)
            {
                combatIndex = 0;
                ExecutingActions = false;
            }
            else if (res == ActionReturns.Ongoing)
            {
                combatIndex++;
            }
            else if(res == ActionReturns.Finished)
            {
                combatIndex = 0;
            }
        }
    }
}
