using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public abstract class StatusEffect
    {
        public int LiveTime { get; set; } = 1;
        protected Unit Target;
        protected StatusEffect(Unit target)
        {
            Console.WriteLine("Applied " + this.GetType() + " to " + target.GetType());
            this.Target = target;
            GameState.StatusEffects.Add(this);
        }

        public virtual void TryApplyEffect()
        {
            LiveTime--;
            if (LiveTime <= 0) OnEffectEnd();
        }

        public virtual void OnEffectEnd()
        {
            Console.WriteLine(this.GetType() + " ended.");
        }
    }

    public class DamageStatusEffect : StatusEffect
    {
        public DamageStatusEffect(Unit target) : base(target)
        {
            LiveTime = 20;
        }

        public override void TryApplyEffect()
        {
            if (LiveTime % 10 == 0)
            {
                Target.Damage(1);
                Console.WriteLine("Damaged " + Target.GetType());
            }

            base.TryApplyEffect();
        }
    }

    public class RootStatusEffect : StatusEffect
    {
        public RootStatusEffect(Unit target) : base(target)
        {
            LiveTime = 30;
        }

        public override void TryApplyEffect()
        {
            Target.Speed = new GameCoordinate(0, 0);
            base.TryApplyEffect();
        }

        public override void OnEffectEnd()
        {
            Target.Speed = Target.InitialSpeed;
            base.OnEffectEnd();
        }
    }
}
