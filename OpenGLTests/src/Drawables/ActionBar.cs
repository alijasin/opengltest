using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    class ActionBar : Element
    {
        public IActionCapable Owner;
        private List<ActionBarButton> actionButtons = new List<ActionBarButton>();

        public int MaxActionSlots = 8;
        public int FilledActionSlots = 0;
        private float fodder = 0.01f;

        public ActionBar(IActionCapable owner)
        {
            this.Location = new GLCoordinate(0, -1);
            this.Size = new GLCoordinate(ActionBarButton.StandardSize.X*MaxActionSlots + fodder*MaxActionSlots, 0.2f + fodder*4); //fodder between each action barButton as well as over and under
            this.Color = Color.Purple;
            this.Owner = owner;
        }

        /// <summary>
        /// If there are slots in the action bar left: adds the action barButton to the action bar. 
        /// Else does nothing.
        /// </summary>
        /// <param name="ab"></param>
        public void Add(Ability s)
        {
            var ab = new ActionBarButton(s, this);
            if (FilledActionSlots < MaxActionSlots)
            {
                ab.OnInteraction += () =>
                {
                    //Owner.ActionHandler.SetActiveAction(ab.GameAction);
                };
                ab.Location = new GLCoordinate(fodder * (FilledActionSlots + 1) + this.Location.X - this.Size.X / 2 + FilledActionSlots * ab.Size.X + ab.Size.X / 2, this.Location.Y + ab.Size.Y / 2 + fodder);
                actionButtons.Add(ab);
                GameState.Drawables.Add(ab);
                FilledActionSlots += 1;
            }
        }


        public void SetActiveButton(ActionBarButton barButton)
        {
            foreach (var ab in actionButtons.Where(but => but != barButton))
            {
                ab.Deactivate();
            }
            barButton.Activate();
        }
    }
    
}
