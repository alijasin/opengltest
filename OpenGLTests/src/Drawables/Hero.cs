﻿using System;
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
        public IActionHandler ActionHandler { get; set; }
        private bool ExecutingActions = false;
        public bool InCombat { get; set; }
        private bool waitingForActionCommit = true;

        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            ActionHandler = new OutOfCombatActionHandler(this);
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
            InCombat = inCombat;
            ActionHandler.Dispose();
            if (InCombat)
            {
                ActionHandler = new CombatActionHandler(this);
            }
            else
            {
                
                ActionHandler = new OutOfCombatActionHandler(this);
            }
        }

        private int actionIndex = 0;
        public void Step()
        {
            var status = ActionHandler.CommitActions(actionIndex);
            if (status == ActionReturns.Placing) return;
            if (status == ActionReturns.AllFinished || status == ActionReturns.Finished) actionIndex = 0;
            else if (status == ActionReturns.Ongoing) actionIndex++;
        }

        //todo refactror this so we dont have literally duplicated code

        public void OutOfCombatStep()
        {

        }
    }
}
