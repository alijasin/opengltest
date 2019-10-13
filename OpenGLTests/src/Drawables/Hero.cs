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
        public CombatActionHandler CombatActionHandler { get; set; }
        public OutOfCombatActionHandler OutOfCombatActionHandler { get; set; }
        private bool ExecutingActions = false;
        public bool InCombat { get; set; }

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            CombatActionHandler = new CombatActionHandler(this);
            OutOfCombatActionHandler = new OutOfCombatActionHandler();
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

            Inventory = new Inventory();
            GameState.Drawables.Add(Inventory);
            Inventory.Add(new GrowingPoition(this));
            Inventory.Add(new RedPotion(this));

        }
        
        public void OutOfCombatStep()
        {
            OutOfCombatActionHandler.DoGameAction();
        }

        private static int index = 0;
        public void CombatStep()
        {
            if (!ExecutingActions) return; //if you decide later than you want to get rid of action confirmation dont do this check.

            ActionReturns res = CombatActionHandler.TickPlacedActions(index);
            if (res == ActionReturns.AllFinished)
            {
                index = 0;
                ExecutingActions = false;
            }
            else if (res == ActionReturns.Ongoing)
            {
                index++;
            }
            else if(res == ActionReturns.Finished)
            {
                index = 0;
            }
        }

    }
}
