using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day_13
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var (x, y) = Part1(out var tracks);
            System.Diagnostics.Debug.Assert(x == 69 && y == 46);
            Console.WriteLine($"Part1 - X,Y coordinates of the first crash: {x}, {y}");

            (x, y) = Part2(tracks);
            System.Diagnostics.Debug.Assert(x == 118 && y == 108);
            Console.WriteLine($"Part1 - X,Y coordinates of the last card: {x}, {y}");
        }

        public static (int x, int y) Part1(out char[][] tracks)
        {
            tracks = File
                .ReadAllLines("input.txt")
                .Select(line => line.ToCharArray())
                .ToArray();
            var carts = GetCarts(tracks).ToList();

            while (true)
            {
                foreach (var cart in carts)
                {
                    cart.Move(tracks);

                    if (carts.Any(c => 
                        c.Location.X == cart.Location.X 
                        && c.Location.Y == cart.Location.Y
                        && c != cart))
                        return (cart.Location.X, cart.Location.Y);
                }
            }
        }

        public static (int x, int y) Part2(char[][] tracks)
        {
            var carts = GetCarts(tracks).ToList();

            while (true)
            {
                foreach (var cart in carts.OrderBy(c => c.Location.Y).ThenBy(c => c.Location.X).ToList())
                {
                    cart.Move(tracks);

                    if (carts.Count() == 1)
                        return (cart.Location.X, cart.Location.Y);

                    var crashed = carts.Where(c =>
                        c.Location.X == cart.Location.X
                        && c.Location.Y == cart.Location.Y
                        && c != cart);

                    if (crashed.Any())
                    {
                        carts.Remove(cart);
                        carts.Remove(crashed.Single());
                    }
                }
            }
        }

        public static IEnumerable<Cart> GetCarts(char[][] tracks)
        {
            for (int y = 0; y < tracks.Length; y++)
            {
                for (int x = 0; x < tracks[0].Length; x++)
                {
                    var cart = tracks[y][x];

                    if (cart == '>' || cart == '<' || cart == '^' || cart == 'v')
                        yield return new Cart(new Point(x, y), cart);
                }
            }
        }
    }

    public class Cart
    {
        public Point Location { get; set; }
        public Direction CurrDirection { get; set; }
        public Turn NextTurn { get; set;}

        public Cart(Point location, char direction)
        {
            CurrDirection = GetDirection(direction);
            Location = location;
            NextTurn = Turn.Left;
        }

        public void Move(char[][] tracks)
        {
            var nextPoint = default(char);

            switch (CurrDirection)
            {
                case Direction.Left:
                    Location = new Point(Location.X - 1, Location.Y);
                    nextPoint = tracks[Location.Y][Location.X];
                    if (nextPoint == '\\') CurrDirection = Direction.Up;
                    if (nextPoint == '/') CurrDirection = Direction.Down;
                    break;
                case Direction.Right:
                    Location = new Point(Location.X + 1, Location.Y);
                    nextPoint = tracks[Location.Y][Location.X];
                    if (nextPoint == '\\') CurrDirection = Direction.Down;
                    if (nextPoint == '/') CurrDirection = Direction.Up;
                    break;
                case Direction.Up:
                    Location = new Point(Location.X, Location.Y - 1);
                    nextPoint = tracks[Location.Y][Location.X];
                    if (nextPoint == '\\') CurrDirection = Direction.Left;
                    if (nextPoint == '/') CurrDirection = Direction.Right;
                    break;
                case Direction.Down:
                    Location = new Point(Location.X, Location.Y + 1);
                    nextPoint = tracks[Location.Y][Location.X];
                    if (nextPoint == '\\') CurrDirection = Direction.Right;
                    if (nextPoint == '/') CurrDirection = Direction.Left;
                    break;
            }

            if (nextPoint == '+')
                TurnCart();
        }

        private void TurnCart()
        {
            if (NextTurn == Turn.Straight)
            {
                NextTurn = Turn.Right;
                return;
            }

            switch (CurrDirection)
            {
                case Direction.Left:
                    CurrDirection = NextTurn == Turn.Left ? Direction.Down : Direction.Up;
                    break;
                case Direction.Right:
                    CurrDirection = NextTurn == Turn.Left ? Direction.Up : Direction.Down;
                    break;
                case Direction.Up:
                    CurrDirection = NextTurn == Turn.Left ? Direction.Left : Direction.Right;
                    break;
                case Direction.Down:
                    CurrDirection = NextTurn == Turn.Left ? Direction.Right : Direction.Left;
                    break;
            }

            NextTurn = NextTurn == Turn.Left ? Turn.Straight : Turn.Left;
        }

        private Direction GetDirection(char dir)
        {
            switch(dir)
            {
                case '>': return Direction.Right;
                case '<': return Direction.Left;
                case '^': return Direction.Up;
                case 'v': return Direction.Down;
                default: throw new Exception("Direction not found: " + dir);
            }
        }
    }

    public enum Turn
    {
        Left, Straight, Right
    }

    public enum Direction
    {
        Left, Right, Up, Down
    }

    public struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() =>
            $"{X}, {Y}";
    }
}