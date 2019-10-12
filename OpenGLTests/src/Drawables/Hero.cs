﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src;

namespace OpenGLTests.src.Drawables
{
    public class Hero : Entity, IActor
    {
        public Inventory Inventory;
        public ActionHandler ActionHandler { get; set; }
        private bool ExecutingActions = false;


        public Hero()
        {
            Color = Color.CadetBlue;
            this.Location = new GameCoordinate(0f, 0f);
            this.Size = new GLCoordinate(0.1f, 0.1f);
            ActionHandler = new ActionHandler(this);
            this.Speed = new GameCoordinate(0.02f, 0.02f);

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
        


        public override void Step()
        {
            if (ExecutingActions)
            {
                CombatStep();
            }
        }

        private static int index = 0;
        public override void CombatStep()
        {
            base.CombatStep();
            ActionHandler.ActionReturns res = ActionHandler.TickPlacedActions(index);
            if (res == ActionHandler.ActionReturns.AllFinished)
            {
                index = 0;
                ExecutingActions = false;
            }
            else if (res == ActionHandler.ActionReturns.Ongoing)
            {
                index++;
            }
            else if(res == ActionHandler.ActionReturns.Finished)
            {
                index = 0;
            }
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
        }
    }
}
