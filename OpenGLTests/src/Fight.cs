using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Drawables.Entities;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public class UnitAndPortrait
    {
        public DrawableButton Portrait;
        public Unit Unit;

        public UnitAndPortrait(Unit u)
        {
            this.Unit = u;
            this.Portrait = new DrawableButton(u.Clone() as Entity);
            this.Portrait.Size = new GLCoordinate(0.1f, 0.1f);
        }
        public void Add(Unit u)
        {
            this.Unit = u;
            this.Portrait = new DrawableButton(u.Clone() as Entity);
            this.Portrait.Size = new GLCoordinate(0.1f, 0.1f);
        }
    }

    public class Fight : Element
    {
        public Queue<UnitAndPortrait> FighterQueue = new Queue<UnitAndPortrait>();
        private float leftMostOfBar => this.Location.X - this.Size.X / 2;
        private float middleOfBar => this.Location.Y + this.Size.Y / 2;

        public Fight()
        {
            this.Size = new GLCoordinate(0.8f, 0.1f);
            this.Location = new GLCoordinate(0, 0.7f);
            this.Animation = new Animation(new SpriteSheet_Floor());
            this.Animation.SetSprite(SpriteID.floor_8);
            this.Animation.IsStatic = true;

            GameState.Drawables.Add(this);
        }

        public List<Hero> Heroes()
        {
            List<Hero> heroes = new List<Hero>();
            heroes.AddRange(FighterQueue.Where(f => f.Unit is Hero).Select(f => f.Unit).Cast<Hero>().ToList());
            return heroes;
        }

        public bool LastManStanding()
        {
            foreach (var toRem in toRemove.ToList())
            {
                FighterQueue = new Queue<UnitAndPortrait>(FighterQueue.Where(s => s.Unit.ID != toRem.ID));
                Console.WriteLine("removing " + toRem);
            }
            toRemove.Clear();

            return FighterQueue.Count <= 1;
        }

        public void AddFighter(Unit u)
        {
            if (FighterQueue.Any(uap => uap.Unit.ID == u.ID)) return;//already exists
            else FighterQueue.Enqueue(new UnitAndPortrait(u));
        }

        public Unit GetCurrentTurn()
        {
            if (FighterQueue.Count < 1) return null;
            return FighterQueue.Peek().Unit;
        }

        public void UnitFinishedTurn(Unit u)
        {
            FighterQueue.Enqueue(FighterQueue.Dequeue());
            u.OnPostTurn();
            FighterQueue.Peek().Unit.OnPreTurn();
            u.ActionStatus = ActionStatus.WaitingForOther;
        }

        private static List<Unit> toRemove = new List<Unit>();
        public static void RemoveFighter(Unit u)
        {
            toRemove.Add(u);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (FighterQueue.Count <= 1) return;
            base.DrawStep(drawer);

            for (int i = 0; i < FighterQueue.Count; i++)
            {
                FighterQueue.ElementAt(i).Portrait.Location = new GLCoordinate(leftMostOfBar + 0.1f * i + 0.1f / 2, middleOfBar - 0.1f / 2);
                FighterQueue.ElementAt(i).Portrait.DrawStep(drawer);
            }
        }
    }
}
