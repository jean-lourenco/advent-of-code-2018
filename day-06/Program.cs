using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day_6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // This solution (although correct) uses too much memory and computational time
            // Maybe use k-d tree? https://en.wikipedia.org/wiki/K-d_tree

            var part1 = Part1(out var matrix);
            System.Diagnostics.Debug.Assert(4475 == part1);
            Console.WriteLine($"Part1 - Number of tiles of the biggest non-infinity area: {part1}");

            var part2 = Part2(matrix);
            System.Diagnostics.Debug.Assert(35237 == part2);
            Console.WriteLine($"Part2 - Number of tiles of the region: {part2}");
        }


        public static int Part1(out Dictionary<char, int>[,] matrix)
        {
            var letters = Enumerable.Range('a', 26)
                .Concat(Enumerable.Range('A', 26))
                .Take(50)
                .Select(x => (char)x);

            matrix = new Dictionary<char, int>[500, 500];

            var points = File
                .ReadAllLines("input.txt")
                .Zip(letters, (i, l) => new Point(i, l));

            for (int x = 0; x < 500; x++)
            {
                for (int y = 0; y < 500; y++)
                {
                    foreach (var point in points)
                    {
                        ref var value = ref matrix[y, x];

                        if (value == null)
                            value = CreateDict();

                        value[point.Letter] = Math.Abs(point.X - x) + Math.Abs(point.Y - y);
                    }
                }
            }

            var infinityLetters = new HashSet<char>();
            var result = CreateDict();

            for (int x = 0; x < 500; x++)
            {
                for (int y = 0; y < 500; y++)
                {
                    var equalDistance = false;

                    var c = matrix[y, x].Aggregate((l1, l2) =>
                    {
                        if (l1.Value == l2.Value)
                        {
                            equalDistance = true;
                            return l1;
                        }
                        else
                        {
                            equalDistance = false;
                            return l1.Value < l2.Value ? l1 : l2;
                        }
                    }).Key;

                    if (IsInfinity(x, y))
                    {
                        if (!infinityLetters.Contains(c))
                        {
                            infinityLetters.Add(c);
                            result[c] = 0;
                        }
                    }

                    if (infinityLetters.Contains(c))
                        continue;

                    if (!equalDistance)
                        result[c]++;
                }
            }

            return result.Aggregate((r1, r2) => r1.Value > r2.Value ? r1 : r2).Value;

            Dictionary<char, int> CreateDict()
            {
                var dict = new Dictionary<char, int>();

                foreach (var letter in letters)
                {
                    dict.Add(letter, 0);
                }

                return dict;
            }

            bool IsInfinity(int x, int y) =>
                x == 0 || y == 0 || x == 499 || y == 499;
        }

        public static int Part2(Dictionary<char, int>[,] matrix)
        {
            var regionSize = 0;

            for (var x = 0; x < matrix.GetLength(0); x++)
            {
                for (var y = 0; y < matrix.GetLength(1); y++)
                {
                    if (matrix[y, x].Sum(k => k.Value) < 10000)
                        regionSize++;
                }
            }

            return regionSize;
        }
    }

    public struct Point
    {
        public char Letter { get; }
        public int X { get; }
        public int Y { get; }

        public Point(string input, char letter)
        {
            var span = input.AsSpan();
            Letter = letter;
            X = int.Parse(span.Slice(0, span.IndexOf(',')));
            Y = int.Parse(span.Slice(span.IndexOf(",") + 2));
        }
    }
}
