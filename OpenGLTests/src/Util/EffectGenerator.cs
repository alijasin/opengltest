using System;
using System.Collections.Generic;
using System.Drawing;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;


public class FadingCircularParticle : Effect
{
    private GLCoordinate originSize = new GLCoordinate(0.05f, 0.05f);
    private GameCoordinate originSpeed = new GameCoordinate(0.001f, 0.001f);
    private int Alpha = 0;

    public FadingCircularParticle(Entity originEntity, GameCoordinate speed)
    {
        this.originSpeed = speed;
        this.Origin = originEntity.Location;
        init();
    }

    private void init()
    {
        this.Alpha = 255;
        this.Color = Color.Purple;
        this.Location = new GameCoordinate(Origin.X, Origin.Y);
        this.Size = new GLCoordinate(originSize.X, originSize.Y);
        this.Speed = new GameCoordinate(originSpeed.X, originSpeed.Y);
        Console.WriteLine(Location);
    }

    public override void DrawStep(DrawAdapter drawer)
    {
        if (this.Alpha == 0)
        {
            init();
        }

        var xd = Location.ToGLCoordinate();
        drawer.FillCircle(xd.X, xd.Y, new GLCoordinate(Size.X, Size.Y), this.Color);
        this.Location.X += 0.01f;
        this.Location.Y += 0.01f;
        //this.Size.X -= 0.001f;
      //  this.Size.Y -= 0.001f;
        this.Alpha--;
        this.Color = Color.FromArgb(Alpha, Color);
    }
}

public class EffectGenerator : Effect
{
    List<Effect> particles = new List<Effect>();
    public void CreateRectangularElement(int n, Entity following, GameCoordinate speed)
    {
        for (int i = 0; i < n; i++)
        {
            particles.Add(new FadingCircularParticle(following, speed));
        }
        GameState.Drawables.Add(this);
    }

    public override void DrawStep(DrawAdapter drawer)
    {
        foreach (var p in particles)
        {
            p.DrawStep(drawer);
        }
    }
}