using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class RainParticle : Entity
    {
        public RainParticle()
        {
            this.Location = new GameCoordinate(RNG.NegativeOrPositiveOne()*RNG.BetweenZeroAndOne(), RNG.NegativeOrPositiveOne() * RNG.BetweenZeroAndOne());
            this.Color = Color.CornflowerBlue;
            this.Size = new GLCoordinate(0.01f, 0.1f);
            this.Speed = new GameCoordinate(0, 0.05f + 0.01f*RNG.BetweenZeroAndOne());
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);

            this.Location += Speed;

            if (Location.Y > 1)
            {
                this.Location.Y = -1;
            }
        }
    }

    public class RainGenerator
    {
        List<RainParticle> rainParticles = new List<RainParticle>();

        public enum RainType
        {
            Heavy,
            Light,

        }

        public RainGenerator(RainType RainType)
        {
            int rainDrops = 0;
            if (RainType == RainType.Heavy) rainDrops = 300;
            else if (RainType == RainType.Light) rainDrops = 100;

            for (int i = 0; i < rainDrops; i++)
            {
                rainParticles.Add(new RainParticle());
            }
        }

        public void Draw(DrawAdapter drawer)
        {
            foreach (var r in rainParticles)
            {
                r.DrawStep(drawer);
            }
        }
    }
}
