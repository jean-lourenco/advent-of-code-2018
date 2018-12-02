using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var part1 = Part1();
            System.Diagnostics.Debug.Assert(7776 == part1);
            Console.WriteLine($"Part1 Checksum: {part1}");

            var part2 = Part2();
            System.Diagnostics.Debug.Assert("wlkigsqyfecjqqmnxaktdrhbz" == part2);
            Console.WriteLine($"Part2 box id: {part2}");
        }

        public static int Part1() 
        {
            var input = File
                .ReadAllText("input.txt")
                .Split(Environment.NewLine);

            var pairs = 0;
            var trios = 0;

            foreach (var word in input)
            {
                var grp = word
                    .GroupBy(l => l)
                    .Select(g => new { Word = g.Key, Count = g.Count()});

                if (grp.Any(g => g.Count == 2))
                    pairs++;

                if (grp.Any(g => g.Count == 3))
                    trios++;
            }

            Console.WriteLine($"Pairs: {pairs}");
            Console.WriteLine($"Trios: {trios}");
            return pairs * trios;
        }
    
        public static string Part2()
        {
            var input = File
                .ReadAllText("input.txt")
                .Split(Environment.NewLine)
                .OrderBy(w => w)
                .ToList();

            for (var i = 1; i < input.Count; i ++)
            {
                var word1 = input[i - 1];
                var word2 = input[i];

                var diff = word1.Zip(word2, (w1, w2) => w1 == w2 ? 0 : 1).Sum();

                if (diff == 1)
                    return string.Join("", word1.Zip(word2, (w1, w2) => w1 == w2 ? w1.ToString() : ""));
            }

            return null;
        }
    }
}