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

        public bool LastManStanding()
        {
            foreach (var toRem in toRemove)
            {
                toRem.Dispose();
                FighterQueue.ToList().Remove(toRem);
            }
            return FighterQueue.Count <= 1;
        }

        public void AddFighter(Unit u)
        {
            FighterQueue.Enqueue(u);
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

        private static List<Unit> toRemove = new List<Unit>();
        public static void RemoveFighter(Unit u)
        {
            toRemove.Add(u);
        }
    }
}
