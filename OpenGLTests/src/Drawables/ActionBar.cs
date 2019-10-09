using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    class ActionButton : Button
    {
        public GameAction GameAction;
        public ActionButton(GameAction ga)
        {
            this.Size = new GLCoordinate(0.1f, 0.1f);
            GameAction = ga;
            OnInteraction += () =>
            {
                
            };
        }
    }

    class ActionBar : Element
    {
        public IActor Owner;
        private List<ActionButton> actionButtons = new List<ActionButton>();

        public int MaxActionSlots = 8;
        public int FilledActionSlots = 0;

        public ActionBar(IActor owner)
        {
            this.Location = new GLCoordinate(0, -1);
            this.Size = new GLCoordinate(0.8f, 0.2f);
            this.Color = Color.Purple;
            this.Owner = owner;
        }

        /// <summary>
        /// If there are slots in the action bar left: adds the action button to the action bar. 
        /// Else does nothing.
        /// </summary>
        /// <param name="ab"></param>
        public void Add(ActionButton ab)
        {
            if (FilledActionSlots < MaxActionSlots)
            {
                ab.OnInteraction += () =>
                {
                    Owner.ActionHandler.SetActiveAction(ab.GameAction);
                };
                ab.Location = new GLCoordinate(this.Location.X - this.Size.X/2 + FilledActionSlots * ab.Size.X + ab.Size.X/2, this.Location.Y + ab.Size.Y /2);
                GameState.Drawables.Add(ab);
                FilledActionSlots += 1;
            }
        }
    }
    
}
