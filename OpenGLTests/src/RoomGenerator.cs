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
        static Walls walls = new Walls();
        public static void RecursiveBacktrackingMaze()
        {

            //init field

            for (int y = -10; y < 10; y++)
            {
                for (int x = -10; x < 10; x++)
                {
                    var snappedPoint = RNG.SnapCoordinate(new GameCoordinate(x * material.Size.X, y * material.Size.Y), new GameCoordinate(material.Size.X, material.Size.Y));
                    Wall wall = new Wall(new GameCoordinate(snappedPoint.X + material.Size.X/2, snappedPoint.Y + material.Size.Y/2));
                    walls.AddWall(wall);
                }
            }

            walls.BreakRandom();
            walls.DamageAroundCurrent();

            //walls.SelectRandomFromDamaged();
            //walls.DamageAroundCurrent();
            //walls.SelectRandomFromDamaged();
            //walls.DamageAroundCurrent();
        }

        public static void Step()
        {
            walls.SelectRandomFromDamaged();
            walls.DamageAroundCurrent();
        }
    }

    class Walls
    {
        private Dictionary<Position, Wall> wallPos = new Dictionary<Position, Wall>();

        private static int currentX = 0;
        private static int currentY = 0;
        public class Position
        {
            private static int currentX;
            private static int currentY;
            public int x;
            public int y;

            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static Position NextPos()
            {
                var pos = new Position(currentX, currentY);
                currentX++;
                if (currentX == 20)
                {
                    currentX = 0;
                    currentY++;
                }
                return pos;
            }
        }

        public void AddWall(Wall w)
        {
            Position p = Position.NextPos();
            wallPos.Add(p, w);
        }

        public void BreakRandom()
        {
            currentX = RNG.IntegerBetween(0, 20);
            currentY = RNG.IntegerBetween(0, 20);

            var wallToBreak = wallPos.First(p => p.Key.x == currentX && p.Key.y == currentY).Value;
            if(wallToBreak != null) wallToBreak.Break();
        }

        public void DamageAroundCurrent()
        {
            var wallsToDamage = WallsAroundPos(currentX, currentY);
            foreach (var wtd in wallsToDamage.Where(w => !w.Value.IsBroken)) wtd.Value.Damage();
        }

        public void SelectRandomFromDamaged()
        {
            var damaged = wallPos.Where(wall => wall.Value.IsDamaged);

            int maxUntouchedAroundDamaged = 0;
            Dictionary<KeyValuePair<Position, Wall>, int> damagedAdjecentUntouched = new Dictionary<KeyValuePair<Position, Wall>, int>();
            foreach (var dkvp in damaged)
            {
                var untouchedCount = WallsAroundPos(dkvp.Key.x, dkvp.Key.y).Count(wall => wall.Value.IsUntouched);
                damagedAdjecentUntouched.Add(dkvp, untouchedCount);
            }

            //todo for loop this shiznit
            int count8 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 8);
            int count7 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 7);
            int count6 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 6);
            int count5 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 5);
            int count4 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 4);
            int count3 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 3);
            int count2 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 2);
            int count1 = damagedAdjecentUntouched.Count(kvp => kvp.Value == 1);

            int prob1 = count1 * SelectionProbability.Prob(1);
            int prob2 = prob1 + count2 * SelectionProbability.Prob(2);
            int prob3 = prob2 + count3 * SelectionProbability.Prob(3);
            int prob4 = prob3 + count4 * SelectionProbability.Prob(4);
            int prob5 = prob4 + count5 * SelectionProbability.Prob(5);
            int prob6 = prob5 + count6 * SelectionProbability.Prob(6);
            int prob7 = prob6 + count7 * SelectionProbability.Prob(7);
            int prob8 = prob7 + count8 * SelectionProbability.Prob(8);

            int rng = RNG.IntegerBetween(0, prob8);
            Console.WriteLine(rng);
            KeyValuePair<Position, Wall> randomedKVP = new KeyValuePair<Position, Wall>();
            if (rng < prob1)
            {
                Console.WriteLine("PROB 1");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 1).ToList()).Key;
            }
            else if (rng < prob2)
            {
                Console.WriteLine("PROB 2");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 2).ToList()).Key;
            }
            else if (rng < prob3)
            {
                Console.WriteLine("PROB 3");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 3).ToList()).Key;
            }
            else if (rng < prob4)
            {
                Console.WriteLine("PROB 4");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 4).ToList()).Key;
            }
            else if (rng < prob5)
            {
                Console.WriteLine("PROB 5");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 5).ToList()).Key;
            }
            else if (rng < prob6)
            {
                Console.WriteLine("PROB 6");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 6).ToList()).Key;
            }
            else if (rng < prob7)
            {
                Console.WriteLine("PROB 7");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 7).ToList()).Key;
            }
            else if (rng < prob8)
            {
                Console.WriteLine("PROB 8");
                randomedKVP = RNG.RandomElement(damagedAdjecentUntouched.Where(kvp => kvp.Value == 8).ToList()).Key;
            }

            currentX = randomedKVP.Key.x;
            currentY = randomedKVP.Key.y;
            randomedKVP.Value.Break();
        }

        public IEnumerable<KeyValuePair<Position, Wall>> WallsAroundPos(int rX, int rY)
        {
            int leftrX = rX - 1;
            int rightrX = rX + 1;
            int aboverY = rY - 1;
            int belowrY = rY + 1;
            return wallPos.Where(p => (p.Key.x == leftrX && p.Key.y == rY) || (p.Key.x == rightrX && p.Key.y == rY) || (p.Key.x == rX && p.Key.y == aboverY) || (p.Key.x == rX && p.Key.y == belowrY));
            //below accepts diagonal 
            //return wallPos.Where(p => ((p.Key.x == leftrX || p.Key.x == rightrX || p.Key.x == rX) && (p.Key.y == rY || p.Key.y == aboverY || p.Key.y == belowrY)) && !(p.Key.x == rX && p.Key.y == rY)); 
        }
    }

    public static class SelectionProbability
    {
        public static int Prob(int adjecentUntouchedCount)
        {
            switch (adjecentUntouchedCount)
            {
                case 8: return 95;
                case 7: return 80;
                case 6: return 70;
                case 5: return 55;
                case 4: return 30;
                case 3: return 15;
                case 2: return 10;
                case 1: return 5;
                default: return 0;
            }
        }
    }

    class Wall
    {
        private Crate crate;
        private GameCoordinate location;

        public bool IsBroken => crate.Color == Color.White;
        public bool IsDamaged => crate.Color == Color.Red;
        public bool IsUntouched => crate.Color == Color.Blue;

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

        public void Damage()
        {
            crate.Color = Color.Red;
        }
    }
}
