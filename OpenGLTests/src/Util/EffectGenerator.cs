using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src.Util
{
    public abstract class EffectGenerator
    {
        public class SmokeEffectGenerator
        {
            List<SmokeParticle> smokeParticles = new List<SmokeParticle>();

            public SmokeEffectGenerator()
            {
                for (int i = 0; i < 20; i++)
                {

                }
            }

            class SmokeParticle : StatusEffect
            {
                SmokeParticle(GameCoordinate origin)
                {

                }
            }
        }
    }
}
