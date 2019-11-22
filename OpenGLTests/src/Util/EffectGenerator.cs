using System;
using System.Collections.Generic;
using System.Drawing;
using OpenGLTests.src;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;


public class FadingCircularParticle : Effect
{
    private GLCoordinate originSize = new GLCoordinate(0.01f, 0.01f);
    private GameCoordinate originSpeed;
    private int Alpha = 0;
    private int angleTrajectory;
    public FadingCircularParticle(Entity originEntity, GameCoordinate speed, Color c)
    {
        this.Location = this.Origin = new GameCoordinate(originEntity.Location.X, originEntity.Location.Y);
        this.originSpeed = speed;
        this.Color = c;
        init();
    }

    private void init()
    {
        angleTrajectory = RNG.IntegerBetween(0, 361);
        this.Alpha =  RNG.IntegerBetween(1, 256);

        this.Location = new GameCoordinate(Origin.X, Origin.Y);
        this.Size = new GLCoordinate(originSize.X, originSize.Y);

    }

    public override void DrawStep(DrawAdapter drawer)
    {
        if (this.Alpha == 0)
        {
            init();
        }

        var xd = Location.ToGLCoordinate();
        drawer.FillCircle(xd.X, xd.Y, new GLCoordinate(Size.X, Size.Y), this.Color);
        this.Location += Speed;
        angleTrajectory += RNG.NegativeOrPositiveOne()*15;
        float xSpeed = originSpeed.X * (float)Math.Cos(MyMath.DegToRad((int)(angleTrajectory)));
        float ySpeed = originSpeed.Y * (float)Math.Sin(MyMath.DegToRad((int)(angleTrajectory)));
        this.Speed = new GameCoordinate(xSpeed, ySpeed);

        if (MyMath.DistanceBetweenTwoPoints(this.Location, Origin) > originSize.X * 5)
        {
            this.Size.X -= originSize.X / 50;
            this.Size.Y -= originSize.Y / 50;
            
            if (Size.X < 0 || Size.Y < 0) init();
        }

        this.Alpha--;



        this.Color = Color.FromArgb(Alpha, Color);

    }
}

public class EffectGenerator : Effect
{
    List<Effect> particles = new List<Effect>();

    public void CreateCircleEffects(int n, Entity following, GameCoordinate speed)
    {
        for (int i = 0; i < n * 3; i++)
        {
            particles.Add(new FadingCircularParticle(following, speed, RNG.RandomColor()));
        }
        GameState.Drawables.Add(this);
    }


    public void CreateCircleEffects(int n, Entity following, GameCoordinate speed, Color c)
    {
        for (int i = 0; i < n*3; i++)
        {
            particles.Add(new FadingCircularParticle(following, speed, c));
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