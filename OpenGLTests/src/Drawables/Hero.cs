using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Entity, IActionCapable, ICombatable
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
            actionBar.Add(new Move(this));
            actionBar.Add(new Yell(this));
            actionBar.Add(new Teleport(this));
            actionBar.Add(new TossBomb(this));

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
            //ActionHandler.TryInvokeCurrentAction(outOfCombatIndex);
        }

        //todo refactror this so we dont have literally duplicated code
        private static int combatIndex = 0;
        public void CombatStep()
        {
            //ActionHandler.TryInvokeCurrentAction(combatIndex);
        }
    }
}
