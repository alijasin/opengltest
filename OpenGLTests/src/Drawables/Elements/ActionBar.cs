using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables.Entities.Equipment;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    public class ActionBar : Element
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
                ab.Location = new GLCoordinate(fodder * (FilledActionSlots + 1) + this.Location.X - this.Size.X / 2 + FilledActionSlots * ab.Size.X + ab.Size.X / 2, this.Location.Y + ab.Size.Y / 2 + fodder);
                actionButtons.Add(ab);
                GameState.Drawables.Add(ab);
                FilledActionSlots += 1;
            }
        }

        private const int MOVE_INDEX = 0;
        public ActionButton GetDefaultButton()
        {
            if (actionButtons.Count == 0) return null;
            return this.actionButtons[MOVE_INDEX]; //this is move action.
        }

        public ActionButton GetActionButton(int index)
        {
            if (FilledActionSlots <= index) return GetDefaultButton();

            return actionButtons.ElementAt(index);
        }

        void Swap(ActionBarButton newButton, ActionBarButton oldButton)
        {
            bool found = false;
            foreach (var ab in actionButtons)
            {
                if (ab == oldButton)
                {
                    found = true;
                    actionButtons[actionButtons.FindIndex(bu => oldButton == bu)] = newButton;
                    newButton.Location = oldButton.Location;
                    GameState.Drawables.Add(newButton);
                    GameState.Drawables.Remove(oldButton);
                    break;
                }
            }

            if(!found) Logger.Write("Tried replacing a button that was not found. @ActionBar Swap()", Logger.LoggingLevel.Error);
        }

        //todo: also include what slot we want to swap with. 
        //right now we are only swapping hard coded style.
        private const int WeaponSlot = 1;
        public void Swap(Weapon w)
        {
            try
            {
                Ability speshal = new WeaponAbility((Hero)Owner, w);
                var ab = new ActionBarButton(speshal, this);
                var other = actionButtons.ElementAt(WeaponSlot);
                Swap(ab, other);
            }
            catch (Exception e)
            {
                Logger.Write("Tried replacing an action bar button for an entity which is not hero. Exception: " + e, Logger.LoggingLevel.Error);
            }
        }
    }
    
}
