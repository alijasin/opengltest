﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Util;

namespace OpenGLTests.src.Drawables
{
    class Splash : Entity
    {
        private GLCoordinate rad;
        private bool decreasing = false;
        GLCoordinate MaxRad = new GLCoordinate(0.02f, 0.02f);
        GLCoordinate MinRad = new GLCoordinate(0, 0);

        public Splash(GameCoordinate location)
        {
            this.Location = location;
            this.Color = Color.CornflowerBlue;
            this.rad = MinRad;
            GameState.Drawables.Add(this);
        }
        public override void DrawStep(DrawAdapter drawer)
        {
            if (decreasing)
            {
                rad.X -= 0.001f;
                rad.Y -= 0.0010f;
            }
            else
            {
                rad.Y += 0.0050f;
                rad.X += 0.0050f;
            }
            if (rad.X > MaxRad.X)
            {
                decreasing = true;

            }
            else if (rad.X <= 0.00000000001f)
            {
                GameState.Drawables.Remove(this);
                return;
            }

            var xdfuckme = Location.ToGLCoordinate();
            drawer.FillCircle(xdfuckme.X, xdfuckme.Y, rad, Color);
        }
    }

    class RainParticle : Entity
    {
        private int TTL = 0;
        public RainParticle()
        {
            this.Location = new GameCoordinate(RNG.NegativeOrPositiveOne() * RNG.BetweenZeroAndOne(), RNG.NegativeOrPositiveOne() * RNG.BetweenZeroAndOne()-0.2f);
            this.Color = Color.CornflowerBlue;
            this.Size = new GLCoordinate(0.01f, 0.05f);
            this.Speed = new GameCoordinate(0, 0.005f + 0.01f * RNG.BetweenZeroAndOne());
            TTL = RNG.IntegerBetween(20, 60);
        }

        public override void DrawStep(DrawAdapter drawer)
        {
            base.DrawStep(drawer);
            TTL--;
            this.Location += Speed;
            //this.Size.Y -= this.Size.Y/TTL;
            if (TTL == 1)
            {
                Splash splash = new Splash(new GameCoordinate(Location.X, Location.Y + Size.Y));
                this.Location.Y = RNG.NegativeOrPositiveOne() * RNG.BetweenZeroAndOne()-0.2f;
                this.Size.Y = 0.05f;
                TTL = RNG.IntegerBetween(20, 60);
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
            Clear,

        }

        public RainGenerator(RainType RainType)
        {
            int rainDrops = 0;
            if (RainType == RainType.Heavy) rainDrops = 200;
            else if (RainType == RainType.Light) rainDrops = 100;
            else if (RainType == RainType.Clear) rainDrops = 0;
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
    #region SideViewRain
    /*
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
    */
    #endregion 
}
