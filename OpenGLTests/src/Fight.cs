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
    public class FightBar : Element
    {
        Queue<DrawableButton> Portraits = new Queue<DrawableButton>();
        private int portraitCount = 0;
        public FightBar()
        {
            this.Size = new GLCoordinate(0.8f, 0.1f);
            this.Location = new GLCoordinate(0, 0.7f);
            this.Animation = new Animation(new SpriteSheet_Floor());
            this.Animation.SetSprite(SpriteID.floor_8);
            this.Animation.IsStatic = true;
            GameState.Drawables.Add(this);
            this.Visible = false;
        }

        private float leftMostOfBar => this.Location.X - this.Size.X / 2;
        private float middleOfBar => this.Location.Y + this.Size.Y / 2;

        public void AddFighter(Unit u)
        {
            var xd = u.Clone() as Entity;
            DrawableButton b = new DrawableButton(xd);
            b.Location = new GLCoordinate(leftMostOfBar + 0.1f * portraitCount + 0.1f/2, middleOfBar - 0.1f/2);
            b.Size = new GLCoordinate(0.1f, 0.1f);
            Portraits.Enqueue(b);
            portraitCount++;

            if (Portraits.Count > 0) this.Visible = true;
        }

        public void RemoveFighter(Unit u)
        {
            foreach (var p in Portraits)
            {
                if (p.Entity.ID == u.ID)
                {
                    Portraits.ToList().Remove(p);
                    p.Dispose();
                    if (Portraits.Count <= 1) this.Visible = false;
                    return;
                }
            }
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            if (this.Visible == false) return;
            base.DrawStep(drawer);
            foreach (var p in Portraits)
            {
                p.DrawStep(drawer);
            }
        }

        public void SetFirstToLast()
        {
            if (Portraits.Count < 1) return;

            for (int i = 0; i < Portraits.Count - 1; i++)
            {
                var tt = this.Portraits.ElementAt(i).Location;
                this.Portraits.ElementAt(i).Location = this.Portraits.ElementAt(i + 1).Location;
                this.Portraits.ElementAt(i+1).Location = tt;
            }
        }
    }

    public class Fight
    {
        FightBar fb = new FightBar();
        Dictionary<Unit, List<GameAction>> fightersAndActions = new Dictionary<Unit, List<GameAction>>();
        public Queue<Unit> FighterQueue = new Queue<Unit>();
        public Fight()
        {
            //we are not sending in anything. we are not sorting anything. this should be done in AddFighter instead.
            //todo
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
                fb.RemoveFighter(toRem);
                toRem.Dispose();
                FighterQueue.ToList().Remove(toRem);
            }
            return FighterQueue.Count <= 1;
        }

        public void AddFighter(Unit u)
        {
            if (FighterQueue.Contains(u)) return;
            FighterQueue.Enqueue(u);
            fb.AddFighter(u);
        }

        public bool TurnToAct(Unit u)
        {
            if (u.ID == fightersAndActions.First().Key.ID) return true;
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
            fb.SetFirstToLast();
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
