﻿using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTests.src.Drawables
{
    public abstract class Drawable
    {
        public bool Visible { get; set; } = true;
        public Color Color { get; set; } = Color.Fuchsia;
        public GLCoordinate Size { get; set; } = new GLCoordinate(0.1f, 0.1f);
        public Animation Animation { get; set; }
        private GameCoordinate location;
        public virtual GameCoordinate Location
        {
            get
            {
                if (this.location == null) return new GameCoordinate(0, 0);
                return this.location;
            }
            set { location = value; }
        }


        public virtual void DrawStep(DrawAdapter drawer)
        {
            drawer.DrawSprite(this, DrawAdapter.DrawMode.Centered);
        }

        public virtual void Dispose()
        {

        }

    }

    public abstract class Entity : Drawable
    {
        public GameCoordinate Speed { get; set; } = new GameCoordinate(0, 0);
        public override void DrawStep(DrawAdapter drawer)
        {
            GLCoordinate location = Location.ToGLCoordinate(GameState.ActiveCamera.Location);

            if (Visible)
            {
                if (Animation == null)
                {
                    drawer.FillRectangle(Color, location.X, location.Y, Size.X, Size.Y);
                }
                else
                {
                    drawer.DrawSprite(this);
                }
            }
        }

        public virtual void CombatStep()
        {

        }

        public virtual void OutOfCombatStep()
        {

        }
    }

    public abstract class Element : Drawable
    {
        public virtual GLCoordinate Location { get; set; } = new GLCoordinate(0, 0);

        public override void DrawStep(DrawAdapter drawer)
        {
            if (Visible) drawer.FillRectangle(Color, Location.X, Location.Y, Size.X, Size.Y);
        }
    }
}
