using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenGLTests.src.Drawables;

namespace OpenGLTests.src
{
    public static class CoordinateFuckery
    {
        public static GameCoordinate ClickToGLRelativeToCamera(GameCoordinate clicked, GameCoordinate entityLocation)
        {
            var x = Math.Abs(clicked.X - entityLocation.X);
            var y = Math.Abs(clicked.Y - entityLocation.Y);
            var gcc = new GLCoordinate(x * 2 / GibbWindow.WIDTH - 1, y * 2 / GibbWindow.HEIGHT - 1);
            return new GameCoordinate(gcc.X + GameState.Players.First().ActiveCamera.Location.X, gcc.Y + GameState.Players.First().ActiveCamera.Location.Y);
        }
    }

    public class GameCoordinate : IEquatable<GameCoordinate>
    {
        public float X { get; set; }
        public float Y { get; set; }
        [JsonConstructor]
        public GameCoordinate(float x, float y)
        {
            X = x;
            Y = y;
        }

        public GameCoordinate Add(float x, float y)
        {
            return new GameCoordinate(X + x, Y + y);
        }

        public GLCoordinate ToGLCoordinate()
        {
            var x = X - GameState.Players.First().ActiveCamera.Location.X;
            var y = -(Y - GameState.Players.First().ActiveCamera.Location.Y);

            return new GLCoordinate(x, y);
        }

        public static GameCoordinate FromGLCoordinate(GLCoordinate raw, GameCoordinate cameraLocation)
        {
            return new GameCoordinate(raw.X + cameraLocation.X, raw.Y + cameraLocation.Y);
        }


        public bool Equals(GameCoordinate other)
        {
            return other != null &&
                   other.X == this.X &&
                   other.Y == this.Y;
        }

        public static GameCoordinate operator +(GameCoordinate a, GameCoordinate b)
        {
            return new GameCoordinate(a.X + b.X, a.Y + b.Y);
        }

        public static GameCoordinate operator -(GameCoordinate a)
        {
            return new GameCoordinate(-a.X, -a.Y);
        }

        public static GameCoordinate operator -(GameCoordinate a, GameCoordinate b)
        {
            return a + -b;
        }

        public bool CloseEnough(GameCoordinate other, float distance)
        {
            return
                Math.Abs(X - other.X) < distance &&
                Math.Abs(Y - other.Y) < distance;
        }

        public GameCoordinate SnapCoordinate()
        {
            return SnapCoordinate(new GameCoordinate(0.1f, 0.1f));
        }

        public GameCoordinate SnapCoordinate(GameCoordinate snapSize)
        {
            var x = Math.Round(this.X / snapSize.X);
            var y = Math.Round(this.Y / snapSize.Y);
            return new GameCoordinate((float)x * snapSize.X, (float)y * snapSize.Y);
        }

        public override string ToString()
        {
            return String.Format("[{0:0.00} {1:0.00}]", X, Y);
        }

        public float Distance(GameCoordinate other)
        {
            var xDistance = Math.Abs(X - other.X);
            var yDistance = Math.Abs(Y - other.Y);
            return (float)Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
        }
    }

    public class GLCoordinate : IEquatable<GLCoordinate>
    {
        public float X { get; set; }
        public float Y { get; set; }

        [JsonConstructor]
        public GLCoordinate(float x, float y)
        {
            X = x;
            Y = y;
        }

        public GLCoordinate(GLCoordinate clonee) : this(clonee.X, clonee.Y)
        {
        }

        public bool Equals(GLCoordinate other)
        {
            return other != null &&
                   other.X == this.X &&
                   other.Y == this.Y;
        }

        public bool CloseEnough(GLCoordinate other, float distance)
        {
            return
                Math.Abs(X - other.X) < distance &&
                Math.Abs(Y - other.Y) < distance;
        }

        public static GLCoordinate operator +(GLCoordinate a, GLCoordinate b)
        {
            return new GLCoordinate(a.X + b.X, a.Y + b.Y);
        }

        public static GLCoordinate operator -(GLCoordinate a)
        {
            return new GLCoordinate(-a.X, -a.Y);
        }

        public static GLCoordinate operator -(GLCoordinate a, GLCoordinate b)
        {
            return a + -b;
        }

        public override string ToString()
        {
            return String.Format("[{0:0.00} {1:0.00}]", X, Y);
        }
    }
}
