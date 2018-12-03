using System;
using System.IO;
using System.Linq;

namespace day_3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1(out var fabric);
            System.Diagnostics.Debug.Assert(111266 == part1);
            Console.WriteLine($"Part 1 - Number of inches overlapped: {part1}");

            var part2 = Part2(fabric);
            System.Diagnostics.Debug.Assert("266" == part2);
            Console.WriteLine($"Part 2 - The only claim that doesn't overlap: {part2}");
        }

        public static int Part1(out int[,] fabric) 
        {
            var inputs = File
                .ReadAllText("input.txt")
                .Split(Environment.NewLine)
                .Select(ParsedInput.Parse);

            var tooManyClaims = 0;
            fabric = new int[1000,1000];

            foreach (var input in inputs)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    for (int y = 0; y < input.Height; y++)
                    {
                        if (fabric[y + input.TopPad, x + input.LeftPad]++ == 1)
                            tooManyClaims++;
                    }
                }
            }

            return tooManyClaims;
        }

        public static string Part2(int[,] fabric)
        {
            var inputs = File
                .ReadAllText("input.txt")
                .Split(Environment.NewLine)
                .Select(ParsedInput.Parse);

            foreach (var input in inputs)
            {
                var overlapped = false;
                for (int x = 0; x < input.Width; x++)
                {
                    for (int y = 0; y < input.Height; y++)
                    {
                        if (fabric[y + input.TopPad, x + input.LeftPad] > 1)
                            overlapped = true;
                    }
                }

                if (!overlapped)
                    return input.Id;
            }

            return null;
        }
    }

    public struct ParsedInput
    {
        public string Id { get; }
        public int LeftPad { get; }
        public int TopPad { get; }
        public int Width { get; }
        public int Height { get; }

        public ParsedInput(string input)
        {
            var span = input.AsSpan();

            Id = span.Slice(1, span.IndexOf(" ") - 1).ToString();

            var index = span.IndexOf('@') + 2;
            LeftPad = int.Parse(span.Slice(index, span.IndexOf(',') - index));

            index = span.IndexOf(',') + 1;
            TopPad = int.Parse(span.Slice(index, span.IndexOf(':') - index));

            index = span.IndexOf(':') + 2;
            Width = int.Parse(span.Slice(index, span.IndexOf('x') - index));

            index = span.IndexOf('x') + 1;
            Height = int.Parse(span.Slice(index));
        }

        public static ParsedInput Parse(string input) => new ParsedInput(input);
    }
}