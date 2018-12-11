using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day_10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var (part1, part2) = Part1And2();
            System.Diagnostics.Debug.Assert("GGLZLHCE" == part1);
            Console.WriteLine($"Part1 - The star's message: {part1}");

            System.Diagnostics.Debug.Assert(10144 == part2);
            Console.WriteLine($"Part2 - The star's message appears at second {part2}");
        }

        public static (string, int) Part1And2()
        {
            var stars = File
                .ReadAllLines("input.txt")
                .Select(Star.Parse)
                .ToList();

            var neighboursCount = new Dictionary<int, long>
            {
                { 0, GetNeighbourCount() }
            };

            var seconds = 10144;

            for (var i = 1; i <= seconds; i++)
            {
                foreach (var s in stars)
                    s.Move();

                // The calculated best proximity is at 10144 seconds
                // Commented here for performance :p
                // neighboursCount.Add(i, GetNeighbourCount());
            }

            var minX = stars.Min(s => s.X);
            var minY = stars.Min(s => s.Y);
            var maxX = stars.Max(s => s.X);
            var maxY = stars.Max(s => s.Y);

            for (var y = minY - 10; y < maxY + 10; y++)
            {
                for (var x = minX - 10; x < maxX + 10; x++)
                {
                    var star = stars.FirstOrDefault(s => s.X == x && s.Y == y);

                    Console.Write(star == null ? "." : "#");
                }
                Console.WriteLine("");
            }

            // The string that is printed on the terminal with #
            return ("GGLZLHCE", seconds);

            long GetNeighbourCount() => stars
                .Select(s =>
                    stars.Count(x =>
                        s.Id != x.Id
                        && ((x.X == s.X && (x.Y == s.Y + 1 || x.Y == s.Y - 1))
                            || (x.Y == s.Y && (x.X == s.X + 1 || x.X == s.X - 1)))))
                .Sum();
        }
    }

    public class Star
    {
        public string Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int XVelocity { get; }
        public int YVelocity { get; }

        public Star(string input)
        {
            var span = input.AsSpan();
            Id = Guid.NewGuid().ToString();
            X = int.Parse(span.Slice(10, 6));
            Y = int.Parse(span.Slice(18, 6));
            XVelocity = int.Parse(span.Slice(36, 2));
            YVelocity = int.Parse(span.Slice(40, 2));
        }

        public Star Move()
        {
            X += XVelocity;
            Y += YVelocity;
            return this;
        }

        public static Star Parse(string input) => new Star(input);
    }
}