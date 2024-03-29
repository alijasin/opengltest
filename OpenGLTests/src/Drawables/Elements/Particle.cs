﻿using System;
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
        private GameCoordinate origin;
        private GLCoordinate size;
        public Particle(GameCoordinate origin)
        {
            this.origin = origin;
            this.size = new GLCoordinate(0.1f, 0.1f);
            init(origin);
        }

        private void init(GameCoordinate origin)
        {
            life = RNG.IntegerBetween(20, 40);
            Speed = new GameCoordinate(RNG.BetweenZeroAndOne() / 100 * RNG.NegativeOrPositiveOne(), (RNG.BetweenZeroAndOne() / 100 * RNG.NegativeOrPositiveOne()));
            Location = origin;
            this.Color = RNG.RandomColor();
            this.Size = new GLCoordinate(size);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            life -= 1;
            this.Size = new GLCoordinate(this.Size.X / 1.1f, this.Size.Y / 1.1f);
            this.Location += Speed;
            if (life < 0)
            {
                init(origin);
            }
        }
    }

    class ParticleGenerator
    {
        private List<Particle> particles;

        public ParticleGenerator(int n, GameCoordinate location)
        {
            particles = new List<Particle>(n);
            for (int i = 0; i < n; i++)
            {
                Particle p = new Particle(location);
                particles.Add(p);
                GameState.Drawables.Add(p);
            }
        }

        public void Draw(DrawAdapter drawer)
        {
            foreach (var p in particles)
            {
                p.DrawStep(drawer);
            }
        }
    }

    //velocity decreases as time goes on.
    public class Particle3 : Entity
    {
        private int life;
        private GameCoordinate origin;


        public Particle3(GameCoordinate origin)
        {

            this.origin = origin;

        }

        public void Init()
        {
            this.Color = RNG.RandomColor();
            Location = origin;
            life = RNG.IntegerBetween(360, 720);
            Speed = new GameCoordinate(RNG.BetweenZeroAndOne() / 100 * RNG.NegativeOrPositiveOne(), (RNG.BetweenZeroAndOne() / 100 * RNG.NegativeOrPositiveOne()));
            this.Size = new GLCoordinate(0.008f, 0.008f);

        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            this.Speed = new GameCoordinate(this.Speed.X / 1.01f, this.Speed.Y/1.01f);
            life -= 1;
            this.Location += Speed;
            if (life < 0)
            {
                Init();
            }
        }
    }

    //circle 
    public class Particle4 : Entity
    {
        private int life;
        private GameCoordinate origin;
        private float rad = 0;
        private int speed = 4;
        private float radInc = 0.005f;
        public Particle4(GameCoordinate origin)
        {

            this.origin = origin;

        }

        public void Init()
        {
            rad = RNG.BetweenZeroAndOne() / 100;
            this.Color = RNG.RandomColor();
            Location = new GameCoordinate(origin.X + rad * RNG.NegativeOrPositiveOne(), origin.Y +rad * RNG.NegativeOrPositiveOne());
            life = RNG.IntegerBetween(360*2, 720*4);
            this.Size = new GLCoordinate(0.008f, 0.008f);

        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            life -= 1;
            this.Location = new GameCoordinate((float)Math.Cos(life* speed * Math.PI/180f)*rad, (float)Math.Sin(life* speed * Math.PI/180f)*rad);
            rad += radInc;

            if (life < 0)
            {
                Init();
                radInc = radInc * -1;
                rad = RNG.BetweenZeroAndOne() / 100;
                Location = new GameCoordinate(origin.X + rad * RNG.NegativeOrPositiveOne(), origin.Y + rad * RNG.NegativeOrPositiveOne());
            }

        }
    }

    public class TestParticleGenerator : Entity
    {
        public TestParticleGenerator(int n)
        {
            n *= 10;
            for (int i = 0; i < n; i++)
            {
                Particle4 p = new Particle4(new GameCoordinate(0, 0));
                GameState.Drawables.Add(p);
                this.Visible = false;
            }
        }
    }


    
}
