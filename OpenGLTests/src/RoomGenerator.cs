using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLTests.src.Drawables;
using OpenGLTests.src.Util;

namespace OpenGLTests.src
{
    public enum Direction
    {
        Up, Down, Left, Right
    }
    public static class RoomGenerator
    {
        static Crate material = new Crate(null);
        public static void RecursiveBacktrackingMaze()
        {
            //List<Wall> walls = new List<Wall>();
            List<Square> squares = new List<Square>();
            Wall[,] walls = new Wall[20,20];
            //init field
            int xx = 0;
            int yy = 0;
            for (int y = -10; y < 10; y++)
            {
                for (int x = -10; x < 10; x++)
                {
                    var snappedPoint = RNG.SnapCoordinate(new GameCoordinate(x * material.Size.X, y * material.Size.Y), new GameCoordinate(material.Size.X, material.Size.Y));
                    Wall wall = new Wall(new GameCoordinate(snappedPoint.X + material.Size.X/2, snappedPoint.Y + material.Size.Y/2));
                    walls[xx, yy]= wall;
                    xx++;
                }

                yy++;
                xx = 0;
            }

            //set initial
            var initial = RNG.RandomElement<Wall>(walls.OfType<Wall>().ToList());
            initial.Break();
            //Choose an arbitrary vertex from G (the graph), and add it to some (initially empty) set V.
            var arbitraryVertexX = RNG.IntegerBetween(0, 20);
            var arbitraryVertexY = RNG.IntegerBetween(0, 20);

            //Choose the edge with the smallest weight from G, that connects a vertex in V with another vertex not in V.


        }

        private static Direction RandomDirection()
        {
            var rand = RNG.IntegerBetween(0, 4);
            switch (rand)
            {
                case 0: return Direction.Left;
                case 1: return Direction.Right;
                case 2: return Direction.Up;
                case 3: return Direction.Down;
                default: throw new Exception("ya goofed");
            }
        }
    }

    class Wall
    {
        private Crate crate;
        private GameCoordinate location;

        public Wall(GameCoordinate location)
        {
            this.location = location;
            this.crate = new Crate(location);
            crate.Color = Color.Blue;
            GameState.Drawables.Add(crate);
        }

        public void Break()
        {
            crate.Color = Color.White;
        }
    }

    class Square
    {
        private Crate crate;
        private GameCoordinate location;
        private bool visited = false;

        public bool IsVisited()
        {
            return visited;
        }

        public void SetVisited()
        {
            crate.Color = Color.Blue;
            this.visited = true;
        }
        
        public Square(GameCoordinate location)
        {
            this.location = location;
            this.crate = new Crate(location);
            GameState.Drawables.Add(crate);
        }
    }
}
