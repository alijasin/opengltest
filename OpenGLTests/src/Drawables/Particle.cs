using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Particle : Entity
    {
        private int life;

        public Particle(GameCoordinate origin)
        {
            life = RNG.IntegerBetween(20, 40);
            Speed = new GameCoordinate(RNG.BetweenZeroAndOne()/100 * RNG.NegativeOrPositiveOne(), (RNG.BetweenZeroAndOne()/100*RNG.NegativeOrPositiveOne()));
            Location = origin;
            this.Color = RNG.RandomColor();
        }

        public override void Draw(DrawAdapter drawer)
        {
            base.Draw(drawer);
        }

        public override void Step()
        {
            base.Step();
            life -= 1;
            this.Size = new GLCoordinate(this.Size.X / 1.1f, this.Size.Y / 1.1f);
            this.Location += Speed;
            if (life < 0)
            {
                GameState.Drawables.Remove(this);
            }
        }
    }

    class ParticleGenerator
    {
        public void GenerateParticles(int n, GameCoordinate location)
        {
            for (int i = 0; i < n; i++)
            {
                Particle p = new Particle(location);
                GameState.Drawables.Add(p);
            }
        }
    }
}
