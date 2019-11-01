using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public class Fight
    {
        Dictionary<Unit, List<GameAction>> fightersAndActions = new Dictionary<Unit, List<GameAction>>();
        public Queue<Unit> FighterQueue = new Queue<Unit>();
        public Fight()
        {
            fightersAndActions.OrderBy(u => u.Key.Initiative);
            foreach (var fighter in fightersAndActions.Keys)
            {
                FighterQueue.Enqueue(fighter);
            }
        }

        public void SubmitActions(Unit u, List<GameAction> actions)
        {
            fightersAndActions.Add(u, actions);
            tryDoActions();
        }

        private void tryDoActions()
        {
            //check if any list of gameaction in unit is null, that means the unit hasnt submitted any actions and we dont proceed.
            

        }

        public void AddFighter(Unit u)
        {
            FighterQueue.Enqueue(u);
        }

        public void RemoveFighter(Unit u)
        {
            FighterQueue.ToList().Remove(u);
        }

        public bool TurnToAct(Unit u)
        {
            if (u == fightersAndActions.First().Key) return true;
            else return false;
        }

        public Unit GetCurrentTurn()
        {
            if (FighterQueue.Count < 1) return null;
            return FighterQueue.First();
        }

        public void UnitFinishedTurn(Unit u)
        {
            FighterQueue.Enqueue(FighterQueue.Dequeue());
            Console.WriteLine("calling " + FighterQueue.Peek() + "'s OnPreTurn" );
            u.OnPostTurn();
            FighterQueue.Peek().OnPreTurn();
        }
    }
}
