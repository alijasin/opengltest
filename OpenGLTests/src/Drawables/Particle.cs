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

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
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

    class Particle2 : Entity
    {
        private GLCoordinate StandardSize = new GLCoordinate(0.4f, 0.4f);
        private Entity following;
        public Particle2(Entity following)
        {
            this.following = following;
            init();

        }

        private void init()
        {

            this.Location = following.Location;
            this.Size = StandardSize;
            this.Color = RNG.RandomColor();
            this.Speed = new GameCoordinate(0, 0);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            this.Size = new GLCoordinate(Size.X / 1.1f, Size.Y/1.1f);
            if (Size.X < 0.1f || Size.Y < 0.1f)
            {
                init();
            }
        }
    }

    class ParticleGenerator2 : Entity
    {
        private Entity following;
        private List<Particle2> particles;
        public ParticleGenerator2(Entity follow, int n)
        {
            following = follow;
            particles = new List<Particle2>(n);
            for (int i = 0; i < n; i++)
            {
                particles.Add(new Particle2(following));
            }
        }

        public override GameCoordinate Location
        {
            get { return following.Location; }
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            foreach (var p in particles.ToArray().Reverse())
            {
                p.DrawStep(drawer);
            }
        }
    }
}
